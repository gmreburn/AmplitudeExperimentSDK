namespace Amplitude
{
    using System;

    public class ExperimentVariant {
        internal ExperimentVariant(string name, string value)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }
    }
}