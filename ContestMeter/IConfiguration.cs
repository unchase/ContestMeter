﻿using System.Collections.ObjectModel;

namespace ContestMeter.Common
{
    public interface IConfiguration
    {
        Task AddNewTask();
        void RemoveTask(Task task);
        DeveloperTool AddNewTool();
        void RemoveTool(DeveloperTool tool);

        ObservableCollection<Task> Tasks { get; }
        ObservableCollection<DeveloperTool> DevTools { get; }
        /// <summary>
        /// Папка, в которой хранятся ответы
        /// </summary>
        string SolutionsFolder { get; }

        string InfoPath { get; }
        string Site { get; set; }

        string ContestType { get; set; }
        string ContestName { get; set; }

        void Save();

        string TestsFolder { get; }
    }
}
