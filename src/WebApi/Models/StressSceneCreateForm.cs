using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class StressSceneCreateForm
    {
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        /// <example>http://localhost/api/tests/get</example>
        public string Url { get; set; }

        /// <summary>
        /// Method
        /// </summary>
        /// <example>GET</example>
        public string Method { get; set; }

        public string Body { get; set; }

        /// <summary>
        /// Thread
        /// </summary>
        /// <example>1</example>
        public int Thread { get; set; } = 1;

        /// <summary>
        /// Connection
        /// </summary>
        /// <example>1</example>
        public int Connection { get; set; } = 1;

        /// <summary>
        /// Duration
        /// </summary>
        /// <example>5</example>
        public int Duration { get; set; } = 5;

        /// <summary>
        /// Unit
        /// </summary>
        /// <example>s</example>
        public string Unit { get; set; } = "s";
    }
}
