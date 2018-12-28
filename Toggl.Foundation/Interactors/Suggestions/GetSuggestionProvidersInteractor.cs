﻿using System;
using System.Collections.Generic;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Diagnostics;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Foundation.Services;
using Toggl.Foundation.Suggestions;
using Toggl.Multivac;

namespace Toggl.Foundation.Interactors.Suggestions
{
    public class GetSuggestionProvidersInteractor : IInteractor<IEnumerable<ISuggestionProvider>>
    {
        private readonly int suggestionCount;
        private readonly IStopwatchProvider stopwatchProvider;
        private readonly ITimeService timeService;
        private readonly ITogglDataSource dataSource;
        private readonly ICalendarService calendarService;
        private readonly IInteractor<IObservable<IThreadSafeWorkspace>> defaultWorkspaceInteractor;

        public GetSuggestionProvidersInteractor(
            int suggestionCount,
            IStopwatchProvider stopwatchProvider,
            ITogglDataSource dataSource,
            ITimeService timeService,
            ICalendarService calendarService,
            IInteractor<IObservable<IThreadSafeWorkspace>> defaultWorkspaceInteractor)
        {
            Ensure.Argument.IsInClosedRange(suggestionCount, 1, 9, nameof(suggestionCount));
            Ensure.Argument.IsNotNull(stopwatchProvider, nameof(stopwatchProvider));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(calendarService, nameof(calendarService));
            Ensure.Argument.IsNotNull(defaultWorkspaceInteractor, nameof(defaultWorkspaceInteractor));

            this.suggestionCount = suggestionCount;
            this.stopwatchProvider = stopwatchProvider;
            this.dataSource = dataSource;
            this.timeService = timeService;
            this.calendarService = calendarService;
            this.defaultWorkspaceInteractor = defaultWorkspaceInteractor;
        }

        public IEnumerable<ISuggestionProvider> Execute()
            => new List<ISuggestionProvider>
            {
                new RandomForestSuggestionProvider(stopwatchProvider, dataSource, timeService, suggestionCount),
                new MostUsedTimeEntrySuggestionProvider(timeService, dataSource, suggestionCount),
                new CalendarSuggestionProvider(timeService, calendarService, defaultWorkspaceInteractor)
            };
    }
}