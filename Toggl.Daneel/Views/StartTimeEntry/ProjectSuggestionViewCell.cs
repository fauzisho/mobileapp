﻿using System;
using System.Reactive.Disposables;
using Foundation;
using MvvmCross.Plugin.Color.Platforms.Ios;
using MvvmCross.UI;
using Toggl.Daneel.Cells;
using Toggl.Daneel.Extensions;
using Toggl.Daneel.Extensions.Reactive;
using Toggl.Foundation.Autocomplete.Suggestions;
using Toggl.Multivac.Extensions;
using UIKit;

namespace Toggl.Daneel.Views
{
    public partial class ProjectSuggestionViewCell : BaseTableViewCell<ProjectSuggestion>
    {
        private const float selectedProjectBackgroundAlpha = 0.12f;

        private const int fadeViewTrailingConstraintWithTasks = 72;
        private const int fadeViewTrailingConstraintWithoutTasks = 16;

        public static readonly string Identifier = nameof(ProjectSuggestionViewCell);
        public static readonly UINib Nib;

        public bool TopSeparatorHidden
        {
            get => TopSeparatorView.Hidden;
            set => TopSeparatorView.Hidden = value;
        }

        public bool BottomSeparatorHidden
        {
            get => BottomSeparatorView.Hidden;
            set => BottomSeparatorView.Hidden = value;
        }

        public CompositeDisposable DisposeBag = new CompositeDisposable();
        public IObservable<ProjectSuggestion> ToggleTasks { get; private set; }

        static ProjectSuggestionViewCell()
        {
            Nib = UINib.FromName(nameof(ProjectSuggestionViewCell), NSBundle.MainBundle);
        }

        protected ProjectSuggestionViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            FadeView.FadeRight = true;
            ClientNameLabel.LineBreakMode = UILineBreakMode.TailTruncation;
            ProjectNameLabel.LineBreakMode = UILineBreakMode.TailTruncation;
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();
            
            DisposeBag.Dispose();
            DisposeBag = new CompositeDisposable();
        }

        protected override void UpdateView()
        {            
            ToggleTasks = ToggleTasksButton.Rx().Tap().SelectValue(Item);

            //Text
            ProjectNameLabel.Text = Item.ProjectName;
            ClientNameLabel.Text = Item.ClientName;
            AmountOfTasksLabel.Text = Item.FormattedNumberOfTasks();

            //Color
            var projectColor = MvxColor.ParseHexString(Item.ProjectColor).ToNativeColor();
            ProjectNameLabel.TextColor = projectColor;
            ProjectDotView.BackgroundColor = projectColor;
            SelectedProjectView.BackgroundColor = Item.Selected
                ? projectColor.ColorWithAlpha(selectedProjectBackgroundAlpha)
                : UIColor.Clear;

            //Visibility
            ToggleTaskImage.Hidden = !Item.HasTasks;
            ToggleTasksButton.Hidden = !Item.HasTasks;

            //Constraints
            FadeViewTrailingConstraint.Constant = Item.HasTasks
                ? fadeViewTrailingConstraintWithTasks
                : fadeViewTrailingConstraintWithoutTasks;
        }
    }
}
