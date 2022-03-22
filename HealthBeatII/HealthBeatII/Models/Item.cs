using System;
using SQLite;

namespace HealthBeatII.Models
{
    public class Item
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
    }

    public class PracticeItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }

        public string Part { get; set; }        // 팔, 어깨, 등, 가슴, 복부, 하체
        public string Description { get; set; }

        public string reserved1 { get; set; }
        public string reserved2 { get; set; }
        public string reserved3 { get; set; }
    }

    public class CombinedPracticeItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PracticeItemList { get; set; }    // , 로 구분된 PracticeItem 의 Id

        public string reserved1 { get; set; }
        public string reserved2 { get; set; }
        public string reserved3 { get; set; }
    }

    public class HistoryItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int BPM { get; set; }
        public int CombinedItemId { get; set; }

        public string reserved1 { get; set; }
        public string reserved2 { get; set; }
        public string reserved3 { get; set; }
    }
}