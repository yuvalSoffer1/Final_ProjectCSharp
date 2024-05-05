
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_ProjectCSharp
{
    public class Student
    {
        public string Name { get; set; }
        public List<Details> Details { get; set; }

        public override string ToString()
        {
            if (Details == null || Details.Count == 0)
                return base.ToString();

            // Using StringBuilder for efficient string concatenation
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

            stringBuilder.Append("Name");
            stringBuilder.Append(" - ");
            stringBuilder.Append(Name);
            stringBuilder.Append(". \n");

            // Iterate through the Details list and append each detail to the StringBuilder
            foreach (var detail in Details)
            {
                if (!detail.ColumnName.Contains("%"))
                {
                    stringBuilder.Append(detail.ColumnName);
                    stringBuilder.Append(" - ");
                    stringBuilder.Append(detail.Detail);
                    stringBuilder.Append(". \n");
                }
            }

            // Remove the trailing newline character
            stringBuilder.Remove(stringBuilder.Length - 1, 1);

            return stringBuilder.ToString();
        }
    }
}
