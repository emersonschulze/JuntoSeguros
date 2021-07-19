
using JuntoSeguros.Business;
using NUnit.Framework;
using Moq;
using Junto.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Junto.Dominio;
using System.Linq;
using System.Collections.Generic;
using System;

namespace JuntoSeguros.Testes.Business
{
    [TestFixture]
    public class SegurosServiceTestes
    {

        //private SegurosService _segurosService;

        //[SetUp]
        //public void SetUp()
        //{
        //    var mockSet = new Mock<DbSet<Seguros>>();
        //    var mockContext = new Mock<BancoDbContext>();
        //    mockContext.Setup(m => m.Seguros).Returns(mockSet.Object);

        //    var service = new _segurosService(mockContext.Object);

        //}
        [Test]
        public void IncluirSeguroAgro()
        {

            var mockSet = new Mock<DbSet<Seguros>>();
            var mockContext = new Mock<BancoDbContext>();

            mockContext.Setup(m => m.Set<Seguros>()).Returns(mockSet.Object);
            var service = new SegurosService(mockContext.Object);


            service.Incluir(new Seguros() { Descricao = "Voltado ao produtor rural", Nome = "Agro", Preco = 100.00 });
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Test]
        public void RetornaTodos()
        {
            var data = new List<Seguros>
            {
                new Seguros { Descricao = "Voltado ao produtor rural", Nome="Agro", Preco=100.00},
                new Seguros { Descricao = "Voltado a residencia", Nome="Residencia", Preco=50.00 },
                new Seguros { Descricao = "Voltado a empresas", Nome="Patrimonial", Preco=70.00},
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Seguros>>();
            mockSet.As<IQueryable<Seguros>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Seguros>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Seguros>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Seguros>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<BancoDbContext>();
            mockContext.Setup(x => x.Set<Seguros>()).Returns(mockSet.Object);

            var service = new SegurosService(mockContext.Object);
            var seguros = service.ListarTodos();

            Assert.AreEqual(3, seguros.Count());
            Assert.IsNotNull(seguros.First(x => x.Nome == "Agro"));

        }
    }
}


