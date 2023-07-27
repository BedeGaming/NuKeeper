using NuKeeper.Abstractions.Configuration;
using NuKeeper.Abstractions.RepositoryInspection;
using System;
using System.Collections.Generic;

namespace NuKeeper.Abstractions.CollaborationPlatform
{
    public interface ICommitWorder
    {
        string MakePullRequestTitle(IReadOnlyCollection<PackageUpdateSet> updates, VersionChange version = VersionChange.None, string ticketNumber = null);

        string MakeCommitMessage(PackageUpdateSet updates, VersionChange version = VersionChange.None, string ticketNumber = null);

        string MakeCommitDetails(IReadOnlyCollection<PackageUpdateSet> updates);
    }
}
