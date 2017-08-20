using System;

namespace WizardBallistics.Core {
    [AttributeUsage(AttributeTargets.Method)]
    public class SolverGeneratorMethodAttribute: Attribute {
        public string Name { get; set; }
        public SolverGeneratorMethodAttribute(string Name) {
            this.Name = Name;
        }
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class SolversFactoryAttribute : Attribute {
        
    }
}
