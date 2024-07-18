#### USER CREATION

resource "aws_iam_access_key" "fast_food_access_key" {
  user = aws_iam_user.fast_food_user.name
}

resource "aws_iam_user" "fast_food_user" {
  name = "fast_food_user"
  path = "/system/"
}

data "aws_iam_policy_document" "fast_food_user_policy" {
  statement {
    effect    = "Allow"
    actions   = ["*"]
    resources = ["*"]
  }
}

data "aws_iam_policy_document" "instance_assume_role_policy" {
  statement {
    actions = ["sts:AssumeRole"]

    principals {
      type        = "Service"
      identifiers = ["lambda.amazonaws.com"]
    }
  }
}

resource "aws_iam_user_policy" "fast_food_user_policy" {
  name   = "fast_food_user_policy"
  user   = aws_iam_user.fast_food_user.name
  policy = data.aws_iam_policy_document.fast_food_user_policy.json
}

resource "aws_iam_policy" "policy" {
  name   = "policy"
  policy = data.aws_iam_policy_document.fast_food_user_policy.json
}

resource "aws_iam_role" "iam_for_lambda" {
  name               = "AdministratorAccess"
  assume_role_policy = data.aws_iam_policy_document.instance_assume_role_policy.json
}

resource "aws_iam_role_policy_attachment" "admin" {
  role       = aws_iam_role.iam_for_lambda.name
  policy_arn = aws_iam_policy.policy.arn
}

### SUBINDO VPC
data "aws_availability_zones" "available" {
  filter {
    name   = "opt-in-status"
    values = ["opt-in-not-required"]
  }
}

module "vpc" {
  source  = "terraform-aws-modules/vpc/aws"
  version = "5.0.0"

  name = "vpc-fast-food-totem"
  cidr                 = "10.0.0.0/16"
  azs                  = data.aws_availability_zones.available.names
  public_subnets       = ["10.0.4.0/24", "10.0.5.0/24", "10.0.6.0/24"]
  enable_dns_hostnames = true
  enable_dns_support   = true
}

resource "aws_db_subnet_group" "rds_subnet" {
  name       = "rds-subnet"
  subnet_ids = module.vpc.public_subnets

  tags = {
    Name = "rds-subnet"
  }
}

resource "aws_security_group" "rds_sg" {
  name_prefix = "rds-"

  vpc_id = module.vpc.vpc_id

  ingress {
    from_port   = 1433
    to_port     = 1433
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"] # Allow inbound traffic from any IP address.
  }

  ingress {
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]

  }

  ingress {
    from_port   = 0
    to_port     = 65535
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]

  }

  egress {
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 1433
    to_port     = 1433
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 65535
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]

  }
}

### SUBINDO RDS

resource "aws_db_instance" "rds-mssql" {
  engine                      = "sqlserver-ex"
  engine_version              = "15.00"
  instance_class              = "db.t3.micro"
  identifier                  = "mydb"
  username                    = "sa"
  password                    = "Fernando9+"

  allocated_storage = 20
  storage_type      = "gp2"

  port = 1433

#   parameter_group_name   = aws_db_parameter_group.pg_rds.name

  vpc_security_group_ids = [aws_security_group.rds_sg.id]
  db_subnet_group_name   = aws_db_subnet_group.rds_subnet.name

  skip_final_snapshot = true
  publicly_accessible = true
}


### ECR

resource "aws_ecr_repository" "HealthMed" {
  name                 = "health_med"
  image_tag_mutability = "MUTABLE"

  encryption_configuration {
    encryption_type = "AES256"
  }

  image_scanning_configuration {
    scan_on_push = true
  }

}

resource "null_resource" "push_image" {
  depends_on = [aws_ecr_repository.HealthMed]

  provisioner "local-exec" {
    command = "docker pull alpine && docker tag alpine:latest ${aws_ecr_repository.HealthMed.repository_url}:latest && aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin ${aws_ecr_repository.HealthMed.repository_url} && docker push ${aws_ecr_repository.HealthMed.repository_url}:latest"
  }
}

###### COGNITO


resource "aws_cognito_user_pool" "HealthMedPatients" {
  name                = "HealthMedPatients"
  username_attributes = ["email"]

  schema {
    name                = "email"
    attribute_data_type = "String"
    mutable             = false
    required            = true
  }

  password_policy {
    minimum_length                   = 11
    require_lowercase                = false
    require_numbers                  = false
    require_symbols                  = false
    require_uppercase                = false
    temporary_password_validity_days = 7
  }
}

resource "aws_cognito_user_pool_client" "HealthMedPatientPool" {
  depends_on      = [aws_cognito_user_pool.HealthMedPatients]
  name            = "HealthMedPatientApi"
  user_pool_id    = aws_cognito_user_pool.HealthMedPatients.id
  generate_secret = false

  supported_identity_providers = ["COGNITO"]
  allowed_oauth_flows          = []
  allowed_oauth_scopes         = []
  explicit_auth_flows          = ["ALLOW_ADMIN_USER_PASSWORD_AUTH", "ALLOW_REFRESH_TOKEN_AUTH"]

  token_validity_units {
    access_token  = "minutes"
    id_token      = "minutes"
    refresh_token = "minutes"

  }
  access_token_validity  = 60
  id_token_validity      = 60
  refresh_token_validity = 60
}

resource "aws_cognito_user_pool" "HealthMedDoctors" {
  name                = "HealthMedDoctors"
  username_attributes = ["email"]

  schema {
    name                = "email"
    attribute_data_type = "String"
    mutable             = false
    required            = true
  }

  password_policy {
    minimum_length                   = 11
    require_lowercase                = false
    require_numbers                  = false
    require_symbols                  = false
    require_uppercase                = false
    temporary_password_validity_days = 7
  }
}

resource "aws_cognito_user_pool_client" "HealthMedDoctorPool" {
  depends_on      = [aws_cognito_user_pool.HealthMedDoctors]
  name            = "HealthMedDoctorApi"
  user_pool_id    = aws_cognito_user_pool.HealthMedDoctors.id
  generate_secret = false

  supported_identity_providers = ["COGNITO"]
  allowed_oauth_flows          = []
  allowed_oauth_scopes         = []
  explicit_auth_flows          = ["ALLOW_ADMIN_USER_PASSWORD_AUTH", "ALLOW_REFRESH_TOKEN_AUTH"]

  token_validity_units {
    access_token  = "minutes"
    id_token      = "minutes"
    refresh_token = "minutes"

  }
  access_token_validity  = 60
  id_token_validity      = 60
  refresh_token_validity = 60
}

##### Cloudwatch

resource "aws_cloudwatch_log_group" "FastFoodUserManagementLogging" {
  name = "/HealthMed/Logging"
}

##### Lambda Function

resource "aws_lambda_function" "HealthMedLamda" {
  depends_on = [null_resource.push_image, aws_ecr_repository.HealthMed]
  environment {
    variables = {
      SQL_CONNECTION   = "Server=${split(":", aws_db_instance.rds-mssql.endpoint)[0]},${aws_db_instance.rds-mssql.port};Database=FastFoodTotem;User Id=sa;Password=Fernando9+;MultipleActiveResultSets=true;TrustServerCertificate=true;"
      AWS_DOCTOR_POOL_ID = aws_cognito_user_pool.HealthMedDoctors.id
      AWS_DOCTOR_CLIENT_ID_COGNITO = aws_cognito_user_pool_client.HealthMedDoctorPool.id
      AWS_PATIENT_POOL_ID = aws_cognito_user_pool.HealthMedPatients.id
      AWS_PATIENT_CLIENT_ID_COGNITO = aws_cognito_user_pool_client.HealthMedPatientPool.id
      ACCESS_KEY = aws_iam_access_key.fast_food_access_key.id
      SECRET_KEY = aws_iam_access_key.fast_food_access_key.secret
      LOG_GROUP = "/HealthMed/Logging"
    }
  }
  package_type  = "Image"
  memory_size   = "500"
  timeout       = 60
  architectures = ["x86_64"]
  function_name = "HealthMedLambda"
  image_uri     = "${aws_ecr_repository.HealthMed.repository_url}:latest"
  role          = aws_iam_role.iam_for_lambda.arn
}
