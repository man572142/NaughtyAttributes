using System;
using System.Collections.Generic;
using UnityEditor;

namespace NaughtyAttributes.Editor
{
    public abstract class PropertyValidatorBase
    {
        public abstract void ValidateProperty(SerializedProperty property);
    }

    public static class ValidatorAttributeExtensions
    {
        private static Dictionary<string, PropertyValidatorBase> _validatorsByAttributeType;

        static ValidatorAttributeExtensions()
        {
            _validatorsByAttributeType = new Dictionary<string, PropertyValidatorBase>();
            _validatorsByAttributeType[nameof(MinValueAttribute)] = new MinValuePropertyValidator();
            _validatorsByAttributeType[nameof(MaxValueAttribute)] = new MaxValuePropertyValidator();
            _validatorsByAttributeType[nameof(RequiredAttribute)] = new RequiredPropertyValidator();
            _validatorsByAttributeType[nameof(ValidateInputAttribute)] = new ValidateInputPropertyValidator();
        }

        public static PropertyValidatorBase GetValidator(this ValidatorAttribute attr)
        {
            PropertyValidatorBase validator;
            if (_validatorsByAttributeType.TryGetValue(attr.GetType().Name, out validator))
            {
                return validator;
            }
            else
            {
                return null;
            }
        }
    }
}
