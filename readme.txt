Changes in this branch reflect the needs of Bede in order to be able to use NuKeeper. These so far have been targeted against the 'org' command of the tool.
The intention was to be able to open multiple PRs against Bede's Git org with the expected SemVer tag and ticket number, as per the requirements of our CI tools.
The work is not finished but has been paused for now.

Changes:
- Version and Ticket Number are now required arguments for the tool
- These are then added to the opened PRs in both PR titles and commits. Example: https://github.com/BedeGaming/Bede.MasterAccountView/pull/142

Example working command:
NuKeeper.exe org BedeGaming <github_token> Minor DX-128 -a 0 -f SingleRepositoryOnly --include "Bede.Configuration" -v detailed --consolidate --includerepos "Bede.MasterAccount*"

TODO:
- Look into splitting the AllowedChange property in 2. Currently it's used for SemVer tag argument but it's also maintaining its original functionality of updating packages based on the value of this allowed version, i.e. supplying "Minor" will prevent package updates to major versions. The latter should remain while the SemVer tag functionality should be separated as a separate property in the form of an argument
- Currently the way these two arguments are implemented means they affect all commands. It's not possible to do a "nukeeper inspect" without supplying a SemVer tag and a Ticket Number even though this command doesn't do anything with PRs. These commands should only be required for commands that actually open PRs like "repo" and "org"
- Look into disabling the global command
- Look into only allowing "SingleRepositoryOnly" for fork mode as the other options which actually fork repos are inapplicable for the Bede org (forking is disabled)
- Look into creating an official GitHub release of this tool once merged, so that users can just download the nukeeper.exe and use it easily 
