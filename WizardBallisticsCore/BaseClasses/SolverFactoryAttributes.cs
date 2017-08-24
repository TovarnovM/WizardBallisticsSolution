using System;

namespace WizardBallistics.Core {
    /// <summary>
    /// Атрибут показывающий, что этот метод может генерировать WBSolver'ы
    /// Шаблон метода Func"WBProjectOptions, WBSolver"
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SolverGeneratorMethodAttribute: Attribute {
        public string Name { get; set; }
        public SolverGeneratorMethodAttribute(string Name) {
            this.Name = Name;
        }
    }

    /// <summary>
    /// Этим атрибутом нужно помечать классы, в которых есть методы для генерации WBSolver'ов
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SolversFactoryAttribute : Attribute {
        
    }
}
