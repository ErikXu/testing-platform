namespace WebApi.Models
{
    public class SceneCreateForm
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string Method { get; set; }

        public string Body { get; set; }

        public int Thread { get; set; }

        public int Connection { get; set; }

        public int Duration { get; set; }

        public string Unit { get; set; }
    }
}
