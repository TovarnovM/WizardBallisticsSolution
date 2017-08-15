using System;

namespace WizardBallisticsCore {
    [AttributeUsage(AttributeTargets.Method)]
    public class SolverGeneratorAttribute: Attribute {
        public string Name { get; set; }
        public SolverGeneratorAttribute(string Name) {
            this.Name = Name;
        }
    }
}
