using Jint;
using Jint.Native;
using Jint.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace InterpretadorFormulas
{
    [TestClass]
    public class InterpretadorFormulasTeste
    {
        [TestMethod]
        [DataRow(3, "*", 3, 9d)]
        [DataRow(3, "+", 3, 6d)]
        [DataRow(3, "/", 3, 1d)]
        [DataRow(3, "-", 3, 0d)]
        [DataRow(3, "*", 3, 9d)]
        [DataRow(49.9, "*", 3, 149.7d)]
        [DataRow(1, "*", 0, 0)]
        [DataRow(1389.97, "*", 189.5, 263399.315)]
        public void ExecutaOperacao(double a, string operacao, double b, double esperado)
        {
            var nameA = nameof(a);
            var nameB = nameof(b);

            var atual = new Engine()
                .SetValue(nameA, a)
                .SetValue(nameB, b)
                .Execute($"{nameA}{operacao}{nameB}").GetCompletionValue().AsNumber();

            Assert.AreEqual(esperado, atual);
        }

        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        [DataRow("(3*3", 9d)]
        [DataRow("3*3)", 9d)]
        [DataRow("(1+(3*3)", 10d)]
        public void FaltaFecharParametroLancaParserException(string formula, double esperado)
        {
            var execucao = new Engine().Execute(formula).GetCompletionValue();

            Assert.IsTrue(execucao.IsUndefined());

            var atual = execucao.AsNumber();
        }

        [TestMethod]
        public void FormulaVaziaLancaExcecao()
        {
            ArgumentException erro = null;

            try
            {
                new Engine().Execute("").GetCompletionValue().AsNumber();
            }
            catch (ArgumentException ex)
            {
                erro = ex;
            }

            Assert.IsNotNull(erro);
        }

        [TestMethod]
        [DataRow(81d, 9d)]
        [DataRow(4d, 2d)]
        [DataRow(576d, 24d)]
        [DataRow(144d, 12d)]
        public void ExecutaRaizQuadrade(double valor, double esperado)
        {
            const string formula = "Math.sqrt(a)";

            var atual = new Engine().SetValue("a", valor).Execute(formula).GetCompletionValue().AsNumber();

            Assert.AreEqual(esperado, atual);
        }

        [TestMethod]
        [DataRow(1, 8, -9, 1d, -9d)]
        [DataRow(1, 4, -21, 3d, -7d)]
        [DataRow(1, -5, 6, 3d, 2d)]
        [DataRow(1, -4, -5, 5d, -1d)]
        [DataRow(4, -33, 8, 8d, 0.25d)]
        public void ExecutaFormulaBhaskara(double a, double b, double c, double esperadox1, double esperadox2)
        {
            const string formula = "((b * -1) {0} Math.sqrt( (b * b) - 4 * a * c) ) / (2 * a)";

            var engine = new Engine().SetValue("a", a).SetValue("b", b).SetValue("c", c);

            var x1 = engine.Execute(string.Format(formula, "+")).GetCompletionValue().AsNumber();
            var x2 = engine.Execute(string.Format(formula, "-")).GetCompletionValue().AsNumber();

            Assert.AreEqual(esperadox1, x1);
            Assert.AreEqual(esperadox2, x2);
        }

        [TestMethod]
        public void RaizQuadradaNegativa()
        {
            const string formula = "Math.sqrt(a)";

            var valor = -36d;
            var esperado = "NaN";

            var execucao = new Engine().SetValue("a", valor).Execute(formula).GetCompletionValue();
            var atual = execucao.ToString();

            Assert.AreEqual(esperado, atual);
        }

        [TestMethod]
        public void ExecutaOrdemCorretaOperador()
        {
            const string formula = "5 - 1 + 4 / 2 * 1";
            var esperado = 6d;

            var atual = new Engine().Execute(formula).GetCompletionValue().AsNumber();

            Assert.AreEqual(esperado, atual);
        }
    }
}
