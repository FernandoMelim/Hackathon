# Hackathon

Esse MVP foi desenvolvido como parte do entreg�vel para o Hackaton do curso 'P�s Tech - Software Architecture', turma SOAT4.

## Pr�-requisitos
* [AWS Cloud](https://aws.amazon.com/)							
	* � necess�rio ter uma conta na AWS para subir a infraestrutura necess�ria para o projeto.
* [Terraform local](https://www.terraform.io/)
	* Para subir a infraesturura mantida na pasta Terraform desse projeto.

* [Google Cloud](https://cloud.google.com/?hl=pt_br)
	* � necess�rio gerar uma chave para acessar a Api do Google Maps.

* [Postman](https://www.postman.com/downloads/) - N�o obrigat�rio, mas tem uma collection pronta na pasta raiz do projeto para facilitar os testes.

## Arquitetura do MVP
![FIAP P�s Tech](./fotos/arch.png)

O MVP foi desenvolvido como um monolito em .NET 8 e publicado como Lambda Function. 
A arquitetura consiste em um API Gateway respons�vel por disponibilizar os endpoints criados na lambda function, onde alguns dos endpoints n�o possuem autoriza��o e outros possuem. 
Essa lambda function � integrada a duas User Pools distintas no Cognito, servi�o da AWS respons�vel por lidar com a autentica��o dos pacientes e m�dicos que usar�o o software. 
Como base de dados, optei por utilizar o SQL Server por conta da rela��o existente entre as entidades do projeto. O banco � publicado no RDS da AWS. 
Utilizei o Google Maps API para realizar o c�lculo entre a dist�ncia do paciente e do m�dico que est� procurando. 
Para salvar os logs de erro da aplica��o, utilizei o CloudWatch da AWS.


## Vari�veis de ambiente

- SQL_CONNECTION: String de conex�o SQL completa, incluindo a base de dados `HealthMed`.
- SQL_CONNECTION_WITHOUT_DB: String de conex�o SQL sem especificar a base de dados.
- AWS_DOCTOR_POOL_ID: ID do pool do Amazon Cognito para m�dicos.
- AWS_DOCTOR_CLIENT_ID_COGNITO: ID do cliente do Amazon Cognito para m�dicos.
- AWS_PATIENT_POOL_ID: ID do pool do Amazon Cognito para pacientes.
- AWS_PATIENT_CLIENT_ID_COGNITO: ID do cliente do Amazon Cognito para pacientes.
- ACCESS_KEY: Chave de acesso para servi�os da AWS.
- SECRET_KEY: Chave secreta para servi�os da AWS.
- LOG_GROUP: Grupo de logs no CloudWatch para a aplica��o `HealthMed`.
- GCP_KEY: Chave de API do Google Cloud Platform.

## Execu��o com docker

Para executar com docker, basta executar o seguinte comando na pasta raiz do projeto para gerar a imagem:

``` docker build -t health_med -f .\src\Presentation\HealthMed.Api\Dockerfile . ```

Para subir o container, basta executar o seguinte comando:

```
docker run -e SQL_CONNECTION="Server=mydb.example.com,1433;Database=HealthMed;User Id=sa;Password=YourPassword123;MultipleActiveResultSets=true;TrustServerCertificate=true;" \
-e AWS_DOCTOR_POOL_ID="us-east-1_exampleDoctorPoolId" \
-e AWS_DOCTOR_CLIENT_ID_COGNITO="exampleDoctorClientId" \
-e AWS_PATIENT_POOL_ID="us-east-1_examplePatientPoolId" \
-e AWS_PATIENT_CLIENT_ID_COGNITO="examplePatientClientId" \
-e ACCESS_KEY="EXAMPLEACCESSKEY" \
-e SECRET_KEY="ExampleSecretKey123456" \
-e LOG_GROUP="/HealthMed/Logging" \
-e SQL_CONNECTION_WITHOUT_DB="Server=mydb.example.com,1433;User Id=sa;Password=YourPassword123;MultipleActiveResultSets=true;TrustServerCertificate=true;" \
-e GCP_KEY="ExampleGCPKey" \
-p 8081:8081 -p 8080:8080 health_med
```

Observa��os: as vari�veis de ambiente presentes no comando acima n�o s�o reais. As reais devem ser obtidas ap�s a execu��o do terraform na pr�rpria AWS.

## Endpoints
Funcionalidades dispon�veis.

### Pacientes

- GET /LoginPatient/AuthenticatePatient?Email=email&Cpf=cpf&Password=password - Respons�vel por autenticar um paciente e retornar o accessToken para poder acessar os outros endpoits dentro de seu contexto.
- GET /Patient/SearchDoctor?patientId=patientId&rating=doctorRating&doctorExpertiseId=expertise&km=kmInNumber - Respons�vel por buscar todos os m�dicos dispon�veis de acordo com o filtro passado como QueryParams.
- POST /Patient/ScheduleAppointment - Endpoint para solicitar o agendamento de uma consulta com um m�dico.

### M�dicos

- GET /LoginDoctor/AuthenticateDoctor?Crm=crm&Password=password - Respons�vel por autenticar um m�dico e retornar o accessToken para poder acessar os outros endpoits dentro de seu contexto.
- POST /Doctor/CreateMedicalAppointmentTime - Respons�vel por cadastrar um novo hor�rio de consulta dispon�vel para o m�dico
- GET /Doctor/GetPendingMedicalAppointments/{{doctorId}} - Respons�vel por retornar todas as consultas pendentes de aprova��o de acordo com o id do m�dico.
- PATCH /Doctor/AcceptAppointment/{{id_solicita��o_consulta}}/{{id_horario_disponivel_medico}} - Respons�vel por fazer o m�dico aceitar uma consulta solicitada por um paciente e retir�-la da lista de hor�rios dispon�veis.

## Publica��o

Para subir a infra, basta acessar a pasta "Terraform", presente na raiz do projeto, e executar os comandos do terraform:

- ``` terraform init ```
- ``` terraform apply ```

Lembrando que para executar local � necessario que esteja logado no CLI da AWS localmente com a ACCESS_KEY e SECRET_KEY corretas.

Ap�s a execu��o do terraform, � necess�rio gerar a chave no Google Cloud e colocar na v�riavel de ambiente "GCP_KEY" na lambda function na AWS.

Para publicar a aplica��o, existe uma GitHub Action criada para isso presente [aqui](https://github.com/FernandoMelim/Hackathon/actions/workflows/deploy.yml).
Essa Action � respons�vel por gerar uma imagem docker, subir ela pra um reposit�rio no ECR e publicar essa imagem na Lambda Function gerada pelo terraform. Para isso funcionar corretamente, � necess�rio que as vari�veis "AWS_ACCESS_KEY_ID" e "AWS_SECRET_ACCESS_KEY" estejam devidamente criadas e configuradas no reposit�rio.
Ap�s esses passos, a aplica��o funcionar� corretamente atrav�s do API Gateway criado pelo terraform. O link do v�deo demonstrando a publica��o e funcionamento da API est� presente no PDF entregue na plataforma.