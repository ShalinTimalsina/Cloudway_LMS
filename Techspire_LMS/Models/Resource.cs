using System;

namespace Techspire_LMS.Models
{
    /// <summary>
    /// A generic downloadable attachment on a Lesson — a code sample, a PCAP
    /// capture, a slide deck, a cheat sheet. Same shape regardless of domain;
    /// ResourceType is free text (e.g. "Code", "PCAP", "Slides", "Document").
    /// </summary>
    public class Resource
    {
        public int      ResourceID   { get; set; }
        public int      LessonID     { get; set; }
        public string   Title        { get; set; }
        public string   FilePath     { get; set; }
        public string   ResourceType { get; set; }
        public DateTime UploadedAt   { get; set; }
    }
}
