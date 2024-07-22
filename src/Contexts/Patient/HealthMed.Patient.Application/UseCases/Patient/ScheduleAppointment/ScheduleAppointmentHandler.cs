using HealthMed.Patient.Domain.Contracts.Repositories;
using HealthMed.Patient.Domain.Entities;
using MediatR;

namespace HealthMed.Patient.Application.UseCases.Patient.ScheduleAppointment
{
    public class ScheduleAppointmentHandler(IPatientRepository patientRepository) : IRequestHandler<ScheduleAppointmentRequest, ScheduleAppointmentResponse>
    {
        public async Task<ScheduleAppointmentResponse> Handle(ScheduleAppointmentRequest request, CancellationToken cancellationToken)
        {
            var patientAppointment = new PatientAppointmentsEntity()
            {
                PatientId = request.patientId,
                IdAvailableDoctorAppointment = request.appointmentId
            };

            await patientRepository.InsertPatientAppointmentRequest(patientAppointment);

            return new ScheduleAppointmentResponse();
        }
    }
}
