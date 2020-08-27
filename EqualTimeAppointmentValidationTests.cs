using System;
using System.Collections.Generic;
using AgendaConsultorioMedico.Business;
using AgendaConsultorioMedico.Data;
using Xunit;

namespace AgendaConsultorioMedico.Tests
{
    public class EqualTimeAppointmentValidationTests
    {
        private readonly EqualTimeAppointmentValidation _appointmentValidation;

        public EqualTimeAppointmentValidationTests()
        {
            _appointmentValidation = new EqualTimeAppointmentValidation();
        }

        [Fact]
        public void CanAddApointment_DeveAdicionarUnicaConsulta()
        {
            // Caso em que n�o existe nenhuma outra consulta registrada

            var expected = true;

            var consulta = new Appointment(1, DateTime.Now, DateTime.Now.AddHours(1), "", 1);

            List<Appointment> listaVazia = new List<Appointment>();

            bool actual = _appointmentValidation.CanAddAppointment(consulta, listaVazia);

            Assert.Equal(actual, expected);
        }

        [Fact]
        public void CanAddAppointment_DeveAdicionarAposOutraConsulta()
        {
            // Caso em que Hor�rio de In�cio da Nova Consulta � depois do Hor�rio de Fim da consulta existente.
            // Ex:
            //   Consulta Existente:
            //      In�cio: 13:00
            //      Fim: 14:00
            //
            //   Consulta Nova:
            //      In�cio: 14:00
            //      Fim: 15:00

            var expected = true;

            var consultaExistente = new Appointment(1, DateTime.Now, DateTime.Now.AddHours(1), "", 1);

            var consultaNova = new Appointment(2, DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), "", 2);

            List<Appointment> listaConsultas = new List<Appointment>
            {
                consultaExistente
            };

            bool actual = _appointmentValidation.CanAddAppointment(consultaNova, listaConsultas);

            Assert.Equal(actual, expected);
        }

        [Fact]
        public void CanAddAppointment_DeveAdicionarAntesOutraConsulta()
        {
            // Caso em que Hor�rio de fim da Nova Consulta � antes do Hor�rio de In�cio da consulta existente.
            // Ex:
            //   Consulta Existente:
            //      In�cio: 13:00
            //      Fim: 14:00
            //
            //   Consulta Nova:
            //      In�cio: 12:00
            //      Fim: 13:00

            var expected = true;

            var agora = DateTime.Now;
            var umaHoraDepois = agora.AddHours(1);
            var duasHorasDepois = agora.AddHours(2);

            var consultaExistente = new Appointment(2, umaHoraDepois, duasHorasDepois, "", 2);

            var consultaNova = new Appointment(1, agora, umaHoraDepois, "", 1);

            List<Appointment> listaConsultas = new List<Appointment>
            {
                consultaExistente
            };

            bool actual = _appointmentValidation.CanAddAppointment(consultaNova, listaConsultas);

            Assert.Equal(actual, expected);
        }

        [Fact]
        public void CanAddAppointment_DeveAdicionarEntreDuasConsultas()
        {
            // Caso em que Hor�rio da nova consulta � entre duas outras consultas.
            // Ex:
            //   Consulta Cedo:
            //      In�cio: 13:00
            //      Fim: 14:00
            //
            //   Consulta Tarde:
            //      In�cio: 15:00
            //      Fim: 16:00
            // 
            //   Consulta Nova:
            //      In�cio: 14:00
            //      Fim: 15:00


            var expected = true;

            var agora = DateTime.Now;
            var umaHoraDepois = agora.AddHours(1);
            var duasHorasDepois = agora.AddHours(2);
            var tresHorasDepois = agora.AddHours(3);


            var consultaTarde = new Appointment(2, duasHorasDepois, tresHorasDepois, "", 2);

            var consultaCedo = new Appointment(1, agora, umaHoraDepois, "", 1);

            List<Appointment> listaConsultas = new List<Appointment>
            {
                consultaCedo,
                consultaTarde
            };

            var novaConsulta = new Appointment(3, umaHoraDepois, duasHorasDepois, "", 3);

            bool actual = _appointmentValidation.CanAddAppointment(novaConsulta, listaConsultas);

            Assert.Equal(actual, expected);
        }

        [Fact]
        public void CanAddAppointment_NaoDeveAdicionarNoMesmoHorario()
        {
            // Caso em que Hor�rio de In�cio e Fim das consultas s�o iguais.
            // Ex:
            //   Consulta 1:
            //      In�cio: 13:00
            //      Fim: 14:00
            //
            //   Consulta 2:
            //      In�cio: 13:00
            //      Fim: 14:00

            var expected = false;

            var consultaExistente = new Appointment(1, DateTime.Now, DateTime.Now.AddHours(1), "", 1);

            var consultaNova = new Appointment(2, DateTime.Now, DateTime.Now.AddHours(1), "", 2);

            List<Appointment> listaConsultas = new List<Appointment>
            {
                consultaExistente
            };

            bool actual = _appointmentValidation.CanAddAppointment(consultaNova, listaConsultas);

            Assert.Equal(actual, expected);
        }

        [Fact]
        public void CanAddAppointment_NaoDeveAdicionarNoHorarioInicioConflitante()
        {
            // Caso em que Hor�rio de In�cio da Nova consulta � antes do Hor�rio de Fim da consulta anterior.
            // Ex:
            //   Consulta Existente:
            //      In�cio: 13:00
            //      Fim: 14:00
            //
            //   Consulta Nova:
            //      In�cio: 13:30
            //      Fim: 14:30

            var expected = false;

            var consultaExistente = new Appointment(1, DateTime.Now, DateTime.Now.AddHours(1), "", 1);

            var consultaNova = new Appointment(2, DateTime.Now.AddHours(0.5), DateTime.Now.AddHours(1.5), "", 2);

            List<Appointment> listaConsultas = new List<Appointment>
            {
                consultaExistente
            };

            bool actual = _appointmentValidation.CanAddAppointment(consultaNova, listaConsultas);

            Assert.Equal(actual, expected);
        }

        [Fact]
        public void CanAddAppointment_NaoDeveAdicionarNoHorarioFimConflitante()
        {
            // Caso em que Hor�rio de Fim da Nova consulta � depois do Hor�rio de In�cio da consulta existente.
            // Ex:
            //   Consulta Existente:
            //      In�cio: 13:00
            //      Fim: 14:00
            //
            //   Consulta Nova:
            //      In�cio: 12:30
            //      Fim: 13:30

            var expected = false;

            var consultaExistente = new Appointment(1, DateTime.Now.AddHours(0.5), DateTime.Now.AddHours(1.5), "", 1);

            var consultaNova = new Appointment(2, DateTime.Now, DateTime.Now.AddHours(1), "", 2);

            List<Appointment> listaConsultas = new List<Appointment>
            {
                consultaExistente
            };

            bool actual = _appointmentValidation.CanAddAppointment(consultaNova, listaConsultas);

            Assert.Equal(actual, expected);
        }
    }
}
