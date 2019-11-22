using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NuKeeper.Abstractions.CollaborationModels;
using NuKeeper.Abstractions.CollaborationPlatform;
using NuKeeper.Abstractions.Configuration;
using NuKeeper.Abstractions.Logging;
using NuKeeper.BitBucket.Models;
using Repository = NuKeeper.Abstractions.CollaborationModels.Repository;
using User = NuKeeper.Abstractions.CollaborationModels.User;

namespace NuKeeper.BitBucket
{
    public class BitbucketPlatform : ICollaborationPlatform
    {
        private readonly INuKeeperLogger _logger;
        private BitbucketRestClient _client;
        private AuthSettings _settings;

        public BitbucketPlatform(INuKeeperLogger logger)
        {
            _logger = logger;
        }

        public void Initialise(AuthSettings settings)
        {
            _settings = settings;
            var httpClient = new HttpClient
            {
                BaseAddress = settings.ApiBase
            };
            _client = new BitbucketRestClient(httpClient, _logger, settings.Username, settings.Token);
        }

        public Task<User> GetCurrentUser()
        {
            return Task.FromResult(new User(_settings.Username, _settings.Username, _settings.Username));
        }

        public async Task OpenPullRequest(ForkData target, PullRequestRequest request, IEnumerable<string> labels)
        {
            var repo = await _client.GetGitRepository(target.Owner, target.Name);
            var req = new PullRequest
            {
                title = request.Title,
                source = new Source
                {
                    branch = new Branch
                    {
                        name = request.Head
                    }
                },
                destination = new Source
                {
                    branch = new Branch
                    {
                        name = request.BaseRef
                    }
                },
                description = request.Body,
                close_source_branch = request.DeleteBranchAfterMerge
            };

            await _client.CreatePullRequest(req, target.Owner, repo.name);
        }

        public async Task<IReadOnlyList<Organization>> GetOrganizations()
        {
            var projects = await _client.GetProjects(_settings.Username);
            return projects
                .Select(project => new Organization(project.name))
                .ToList();
        }

        public async Task<IReadOnlyList<Repository>> GetRepositoriesForOrganisation(string projectName)
        {
            var repos = await _client.GetGitRepositories(projectName);
            return repos.Select(MapRepository)
                .ToList();
        }

        public async Task<Repository> GetUserRepository(string projectName, string repositoryName)
        {
            var repo = await _client.GetGitRepository(projectName, repositoryName);
            if (repo == null) return default;
            return MapRepository(repo);
        }

        public Task<Repository> MakeUserFork(string owner, string repositoryName)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RepositoryBranchExists(string projectName, string repositoryName, string branchName)
        {
            var repo = await _client.GetGitRepository(projectName, repositoryName);
            var refs = await _client.GetRepositoryRefs(projectName, repo.name);
            var count = refs.Count(x => x.Name.Equals(branchName, StringComparison.OrdinalIgnoreCase));
            if (count > 0)
            {
                _logger.Detailed($"Branch found for {projectName} / {repositoryName} / {branchName}");
                return true;
            }
            _logger.Detailed($"No branch found for {projectName} / {repositoryName} / {branchName}");
            return false;
        }

        public Task<SearchCodeResult> Search(SearchCodeRequest search)
        {
            throw new NotImplementedException();
        }

        private static Repository MapRepository(BitBucket.Models.Repository repo)
        {
            return new Repository(repo.name, false,
                    new UserPermissions(true, true, true),
                    new Uri(repo.links.html.href),
                    null, false, null);
        }
    }
}
