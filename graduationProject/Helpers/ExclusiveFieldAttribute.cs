using System.ComponentModel.DataAnnotations;

namespace graduationProject.Helpers
{
    public class ExclusiveFieldAttribute : ValidationAttribute
    {
        private readonly string[] dependentProperties;

        public ExclusiveFieldAttribute(params string[] dependentProperties)
        {
            this.dependentProperties = dependentProperties;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            foreach (var dependentProperty in dependentProperties)
            {
                var dependentPropertyValue = validationContext.ObjectType.GetProperty(dependentProperty)?.GetValue(validationContext.ObjectInstance);

                if (value == null && dependentPropertyValue == null)
                {
                    return new ValidationResult($"Either {validationContext.MemberName} or {dependentProperty} should be entered, not both.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
