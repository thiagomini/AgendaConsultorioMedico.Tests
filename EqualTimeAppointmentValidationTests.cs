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
            // Caso em que não existe nenhuma outra consulta registrada

            var expected = true;

            var consulta = new Appointment(1, DateTime.Now, DateTime.Now.AddHours(1), "", 1);

            List<Appointment> listaVazia = new List<Appointment>();

            bool actual = _appointmentValidation.CanAddAppointment(consulta, listaVazia);

            Assert.Equal(actual, expected);
        }

        [Fact]
        public void CanAddAppointment_DeveAdicionarAposOutraConsulta()
        {
            // Caso em que Horário de Início da Nova Consulta é depois do Horário de Fim da consulta existente.
            // Ex:
            //   Consulta Existente:
            //      Início: 13:00
            //      Fim: 14:00
            //
            //   Consulta Nova:
            //      Início: 14:00
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
            // Caso em que Horário de fim da Nova Consulta é antes do Horário de Início da consulta existente.
            // Ex:
            //   Consulta Existente:
            //      Início: 13:00
            //      Fim: 14:00
            //
            //   Consulta Nova:
            //      Início: 12:00
            //      Fim: 13:00

            var expected = true;

            var consultaExistente = new Appointment(2, DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), "", 2);

            var consultaNova = new Appointment(1, DateTime.Now, DateTime.Now.AddHours(1), "", 1);

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
            // Caso em que Horário da nova consulta é entre duas outras consultas.
            // Ex:
            //   Consulta Cedo:
            //      Início: 13:00
            //      Fim: 14:00
            //
            //   Consulta Tarde:
            //      Início: 15:00
            //      Fim: 16:00
            // 
            //   Consulta Nova:
            //      Início: 14:00
            //      Fim: 15:00


            var expected = true;

            var consultaTarde = new Appointment(2, DateTime.Now.AddHours(2), DateTime.Now.AddHours(3), "", 2);

            var consultaCedo = new Appointment(1, DateTime.Now, DateTime.Now.AddHours(1), "", 1);

            List<Appointment> listaConsultas = new List<Appointment>
            {
                consultaCedo,
                consultaTarde
            };

            var novaConsulta = new Appointment(3, DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), "", 3);

            bool actual = _appointmentValidation.CanAddAppointment(novaConsulta, listaConsultas);

            Assert.Equal(actual, expected);
        }

        [Fact]
        public void CanAddAppointment_NaoDeveAdicionarNoMesmoHorario()
        {
            // Caso em que Horário de Início e Fim das consultas são iguais.
            // Ex:
            //   Consulta 1:
            //      Início: 13:00
            //      Fim: 14:00
            //
            //   Consulta 2:
            //      Início: 13:00
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
            // Caso em que Horário de Início da Nova consulta é antes do Horário de Fim da consulta anterior.
            // Ex:
            //   Consulta Existente:
            //      Início: 13:00
            //      Fim: 14:00
            //
            //   Consulta Nova:
            //      Início: 13:30
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
            // Caso em que Horário de Fim da Nova consulta é depois do Horário de Início da consulta existente.
            // Ex:
            //   Consulta Existente:
            //      Início: 13:00
            //      Fim: 14:00
            //
            //   Consulta Nova:
            //      Início: 12:30
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
