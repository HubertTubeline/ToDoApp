﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using ToDoApp.Activities.Interfaces;
using ToDoApp.Common.Models;
using ToDoApp.TaskListView;

namespace ToDoApp.Presenters
{
    public class MainPresenter : BasePresenter
    {
        private readonly IMainView _view;

        private List<UserTaskListModel> _lists;
        private UserTaskListModel _currentList;

        public MainPresenter(IMainView view)
        {
            _view = view;
            _lists = TaskListService.GetAllTaskLists();

            _view.ShowUserInfo(User.Email, User.FirstName + " " + User.LastName);

            if (_lists.Count > 0)
                _currentList = _lists[0];

            UpdateView();
        }

        public void ItemPressed(IMenuItem item)
        {
            if (item.TitleFormatted.ToString() == "New list")
            {
                _view.ShowCreateListDialog();
                return;
            }

            _currentList = _lists.FirstOrDefault(x => x.Name == item.TitleFormatted.ToString());
            if (_currentList == null) return;

            Update();
        }

        public void CreateList(string listName)
        {
            var newList = new UserTaskListModel() {Name = listName};
            TaskListService.CreateTaskList(newList);
            _currentList = newList;
            Update();
        }

        public void EditTask(UserTaskModel taskModel)
        {
            TaskService.UpdateTask(taskModel);
            Update();
        }

        public void CreateTask(UserTaskModel taskModel)
        {
            TaskService.CreateTask(_currentList, taskModel);
            Update();
        }

        public void ChangeTaskCompleted(UserTaskModel item)
        {
            item.Checked = !item.Checked;
            TaskService.UpdateTask(item);
            _view.ShowTaskLists(_lists);
        }

        public void DeleteTask(UserTaskModel item)
        {
            TaskService.DeleteTask(item);
            Update();
        }

        private void Update()
        {
            UpdateData();
            UpdateView();
        }

        private void UpdateData()
        {
            _lists = TaskListService.GetAllTaskLists();

            _currentList = TaskListService.GetTaskList(_currentList.Name);
        }

        private void UpdateView()
        {
            _view.ShowTaskLists(_lists);
            _view.ShowTasks(_currentList);
        }

        /// <summary>
        /// Delete current list
        /// </summary>
        public void DeleteList()
        {
            TaskListService.DeleteTaskList(_currentList);
            _currentList = _lists[0] ?? new UserTaskListModel();

            Update();
        }

        /// <summary>
        /// Update task list and set current list to that
        /// </summary>
        /// <param name="taskList"></param>
        public void EditTaskList(UserTaskListModel taskList)
        {
            TaskListService.UpdateTaskList(taskList);
            _currentList = taskList;
            Update();
        }

        /// <summary>
        /// Calls edit dialog with current list
        /// </summary>
        public void EditListRequest()
        {
            _view.ShowEditListDialog(_currentList);
        }

        /// <summary>
        /// Calls alert "Are you sure" for deleting current list
        /// </summary>
        public void DeleteListRequest()
        {
            _view.ShowDeleteListAlert();
        }

        /// <summary>
        /// Calls edit dialog with task
        /// </summary>
        public void EditTaskRequest(UserTaskModel task)
        {
            _view.ShowEditTaskDialog(task);
        }

        /// <summary>
        /// Calls dialog for create new task
        /// </summary>
        public void CreateTaskRequest()
        {
            _view.ShowCreateTaskDialog();
        }
    }
}