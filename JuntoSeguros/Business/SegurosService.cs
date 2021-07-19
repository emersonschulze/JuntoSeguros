using Junto.Dominio;
using Junto.Infra.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JuntoSeguros.Business
{
    public class SegurosService
    {
        private BancoDbContext _context;

        public SegurosService(BancoDbContext context)
        {
            _context = context;
        }

        public Seguros Obter(int id)
        {
            if (id.Equals(null))
                return null;
            else
            {
                return _context.Seguros.Where(
                    p => p.Id == id).FirstOrDefault();
            }
        }

        public IEnumerable<Seguros> ListarTodos()
        {
            return _context.Seguros
                .OrderBy(p => p.Nome).ToList();
        }

        public Resultado Incluir(Seguros dadosSeguro)
        {
            Resultado resultado = DadosValidos(dadosSeguro);
            resultado.Acao = "Inclusão de Seguros";

            if (resultado.Inconsistencias.Count == 0 &&
                _context.Seguros.Where(
                p => p.Id == dadosSeguro.Id).Count() > 0)
            {
                resultado.Inconsistencias.Add(
                    "Código já cadastrado");
            }

            if (resultado.Inconsistencias.Count == 0)
            {
                _context.Seguros.Add(dadosSeguro);
                _context.SaveChanges();
            }

            return resultado;
        }

        public Resultado Atualizar(Seguros dadosSeguro)
        {
            Resultado resultado = DadosValidos(dadosSeguro);
            resultado.Acao = "Atualização de Seguros";

            if (resultado.Inconsistencias.Count == 0)
            {
                Seguros Seguros = _context.Seguros.Where(
                    p => p.Id == dadosSeguro.Id).FirstOrDefault();

                if (Seguros == null)
                {
                    resultado.Inconsistencias.Add(
                        "Seguros não encontrado");
                }
                else
                {
                    Seguros.Nome = dadosSeguro.Nome;
                    Seguros.Preco = dadosSeguro.Preco;
                    _context.SaveChanges();
                }
            }

            return resultado;
        }

        public Resultado Excluir(int id)
        {
            Resultado resultado = new Resultado();
            resultado.Acao = "Exclusão de Seguros";

            Seguros Seguros = Obter(id);
            if (Seguros == null)
            {
                resultado.Inconsistencias.Add(
                    "Seguros não encontrado");
            }
            else
            {
                _context.Seguros.Remove(Seguros);
                _context.SaveChanges();
            }

            return resultado;
        }

        private Resultado DadosValidos(Seguros Seguros)
        {
            var resultado = new Resultado();
            if (Seguros == null)
            {
                resultado.Inconsistencias.Add(
                    "Preencha os Dados do Seguros");
            }
            else
            {
                if (String.IsNullOrWhiteSpace(Seguros.Nome))
                {
                    resultado.Inconsistencias.Add(
                        "Preencha o Nome do Seguro");
                }
                if (Seguros.Preco <= 0)
                {
                    resultado.Inconsistencias.Add(
                        "O Preço do Seguros deve ser maior do que zero");
                }
            }

            return resultado;
        }
    }
}