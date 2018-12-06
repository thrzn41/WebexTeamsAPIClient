/* 
 * MIT License
 * 
 * Copyright(c) 2018 thrzn41
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thrzn41.WebexTeams.Version1.Admin
{

    /// <summary>
    /// Cisco Webex Teams Admin API Client for API version 1.
    /// </summary>
    public class TeamsAdminAPIClient : TeamsAPIClient
    {

        /// <summary>
        /// Teams organizations API Path.
        /// </summary>
        protected static readonly string TEAMS_ORGANIZATION_API_PATH = GetAPIPath("organizations");

        /// <summary>
        /// Teams licenses API Path.
        /// </summary>
        protected static readonly string TEAMS_LICENSE_API_PATH = GetAPIPath("licenses");

        /// <summary>
        /// Teams roles API Path.
        /// </summary>
        protected static readonly string TEAMS_ROLE_API_PATH = GetAPIPath("roles");

        /// <summary>
        /// Teams events API Path.
        /// </summary>
        protected static readonly string TEAMS_EVENT_API_PATH = GetAPIPath("events");

        /// <summary>
        /// Teams Resource Group API Path.
        /// </summary>
        protected static readonly string TEAMS_RESOURCE_GROUP_API_PATH = GetAPIPath("resourceGroups");

        /// <summary>
        /// Teams Resource Group Membership API Path.
        /// </summary>
        protected static readonly string TEAMS_RESOURCE_GROUP_MEMBERSHIP_API_PATH = GetAPIPath("resourceGroup/memberships");


        /// <summary>
        /// Teams organizations API Uri.
        /// </summary>
        protected static readonly Uri TEAMS_ORGANIZATION_API_URI = new Uri(TEAMS_ORGANIZATION_API_PATH);

        /// <summary>
        /// Teams licenses API Uri.
        /// </summary>
        protected static readonly Uri TEAMS_LICENSE_API_URI = new Uri(TEAMS_LICENSE_API_PATH);

        /// <summary>
        /// Teams roles API Uri.
        /// </summary>
        protected static readonly Uri TEAMS_ROLE_API_URI = new Uri(TEAMS_ROLE_API_PATH);

        /// <summary>
        /// Teams events API Uri.
        /// </summary>
        protected static readonly Uri TEAMS_EVENT_API_URI = new Uri(TEAMS_EVENT_API_PATH);

        /// <summary>
        /// Teams Resource Group API Uri.
        /// </summary>
        protected static readonly Uri TEAMS_RESOURCE_GROUP_API_URI = new Uri(TEAMS_RESOURCE_GROUP_API_PATH);

        /// <summary>
        /// Teams Resource Group Membership API Uri.
        /// </summary>
        protected static readonly Uri TEAMS_RESOURCE_GROUP_MEMBERSHIP_API_URI = new Uri(TEAMS_RESOURCE_GROUP_MEMBERSHIP_API_PATH);




        /// <summary>
        /// Constructor of TeamsAdminAPIClient.
        /// </summary>
        /// <param name="token">token of Teams API.</param>
        /// <param name="retryExecutor">Executor for retry.</param>
        /// <param name="retryNotificationFunc">Notification func for retry.</param>
        internal TeamsAdminAPIClient(string token, TeamsRetry retryExecutor = null, Func<TeamsResultInfo, int, bool> retryNotificationFunc = null)
            : base(token, retryExecutor, retryNotificationFunc)
        {
        }




        #region Person APIs

        /// <summary>
        /// Creates a person.
        /// </summary>
        /// <param name="email">Email addresses of the person.</param>
        /// <param name="displayName">Full name of the person.</param>
        /// <param name="firstName">First name of the person.</param>
        /// <param name="lastName">Last name of the person.</param>
        /// <param name="avatarUri">URL to the person's avatar in PNG format.</param>
        /// <param name="organizationId">ID of the organization to which this person belongs.</param>
        /// <param name="roles">Roles of the person</param>
        /// <param name="licenses">Licenses allocated to the person</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Person> > CreatePersonAsync(string email, string displayName = null, string firstName = null, string lastName = null, Uri avatarUri = null, string organizationId = null, IEnumerable<string> roles = null, IEnumerable<string> licenses = null, CancellationToken? cancellationToken = null)
        {
            List<string> roleList    = null;
            List<string> licenseList = null;

            if(roles != null)
            {
                roleList = new List<string>();

                foreach (var item in roles)
                {
                    roleList.Add(item);
                }
            }

            if (licenses != null)
            {
                licenseList = new List<string>();

                foreach (var item in licenses)
                {
                    licenseList.Add(item);
                }
            }


            var person = new Person();

            person.Emails         = new[] { email };
            person.DisplayName    = displayName;
            person.FirstName      = firstName;
            person.LastName       = lastName;
            person.Avatar         = avatarUri?.AbsoluteUri;
            person.OrganizationId = organizationId;
            person.Roles          = roleList?.ToArray();
            person.Licenses       = licenseList?.ToArray();

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<Person>, Person>(
                                    HttpMethod.Post,
                                    TEAMS_PERSON_API_URI,
                                    null,
                                    person,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Updates a person.
        /// </summary>
        /// <param name="person"><see cref="Person"/> to be updated.</param>
        /// <param name="email">Email addresses of the person.</param>
        /// <param name="displayName">Full name of the person.</param>
        /// <param name="firstName">First name of the person.</param>
        /// <param name="lastName">Last name of the person.</param>
        /// <param name="avatarUri">URL to the person's avatar in PNG format.</param>
        /// <param name="organizationId">ID of the organization to which this person belongs.</param>
        /// <param name="roles">Roles of the person</param>
        /// <param name="licenses">Licenses allocated to the person</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Person> > UpdatePersonAsync(Person person, string email = null, string displayName = null, string firstName = null, string lastName = null, Uri avatarUri = null, string organizationId = null, IEnumerable<string> roles = null, IEnumerable<string> licenses = null, CancellationToken? cancellationToken = null)
        {
            List<string> roleList    = null;
            List<string> licenseList = null;

            if (roles != null)
            {
                roleList = new List<string>();

                foreach (var item in roles)
                {
                    roleList.Add(item);
                }
            }

            if (licenses != null)
            {
                licenseList = new List<string>();

                foreach (var item in licenses)
                {
                    licenseList.Add(item);
                }
            }


            var updatedPerson = new Person();

            updatedPerson.Emails         = new[] { getValueOrDefault(email, person.Emails[0]) };
            updatedPerson.DisplayName    = getValueOrDefault(displayName,            person.DisplayName);
            updatedPerson.FirstName      = getValueOrDefault(firstName,              person.FirstName);
            updatedPerson.LastName       = getValueOrDefault(lastName,               person.LastName);
            updatedPerson.Avatar         = getValueOrDefault(avatarUri?.AbsoluteUri, person.Avatar);
            updatedPerson.OrganizationId = getValueOrDefault(organizationId,         person.OrganizationId);
            updatedPerson.Roles          = getValueOrDefault(roleList?.ToArray(),    person.Roles);
            updatedPerson.Licenses       = getValueOrDefault(licenseList?.ToArray(), person.Licenses);

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<Person>, Person>(
                                    HttpMethod.Put,
                                    new Uri(String.Format("{0}/{1}", TEAMS_PERSON_API_PATH, Uri.EscapeDataString(person.Id))),
                                    null,
                                    updatedPerson,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Deletes a person.
        /// </summary>
        /// <param name="personId">Person id to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<NoContent> > DeletePersonAsync(string personId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<NoContent>, NoContent>(
                                    HttpMethod.Delete,
                                    new Uri(String.Format("{0}/{1}", TEAMS_PERSON_API_PATH, Uri.EscapeDataString(personId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.NoContent));

            return result;
        }

        /// <summary>
        /// Deletes a person.
        /// </summary>
        /// <param name="person"><see cref="Person"/> to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task<TeamsResult<NoContent>> DeletePersonAsync(Person person, CancellationToken? cancellationToken = null)
        {
            return (DeletePersonAsync(person.Id, cancellationToken));
        }

        #endregion


        #region Organization APIs

        /// <summary>
        /// Lists organizations. 
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsListResult<OrganizationList> > ListOrganizationsAsync(CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsListResult<OrganizationList>, OrganizationList>(
                                    HttpMethod.Get,
                                    TEAMS_ORGANIZATION_API_URI,
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Gets organization detail.
        /// </summary>
        /// <param name="organizationId">Organization id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Organization> > GetOrganizationAsync(string organizationId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<Organization>, Organization>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", TEAMS_ORGANIZATION_API_PATH, Uri.EscapeDataString(organizationId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets organization detail.
        /// </summary>
        /// <param name="organization"><see cref="Organization"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task<TeamsResult<Organization>> GetOrganizationAsync(Organization organization, CancellationToken? cancellationToken = null)
        {
            return (GetOrganizationAsync(organization.Id, cancellationToken));
        }

        #endregion


        #region License APIs

        /// <summary>
        /// Lists licenses. 
        /// </summary>
        /// <param name="organizationId">Specify the organization.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsListResult<LicenseList> > ListLicensesAsync(string organizationId = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("orgId", organizationId);

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsListResult<LicenseList>, LicenseList>(
                                    HttpMethod.Get,
                                    TEAMS_LICENSE_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Lists licenses. 
        /// </summary>
        /// <param name="organization">Specify the <see cref="Organization"/>.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsListResult<LicenseList> > ListLicensesAsync(Organization organization, CancellationToken? cancellationToken = null)
        {
            return (ListLicensesAsync(organization.Id, cancellationToken));
        }


        /// <summary>
        /// Gets license detail.
        /// </summary>
        /// <param name="licenseId">License id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<License> > GetLicenseAsync(string licenseId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<License>, License>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", TEAMS_LICENSE_API_PATH, Uri.EscapeDataString(licenseId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets license detail.
        /// </summary>
        /// <param name="license"><see cref="License"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<License> > GetLicenseAsync(License license, CancellationToken? cancellationToken = null)
        {
            return (GetLicenseAsync(license.Id, cancellationToken));
        }

        #endregion


        #region Role APIs

        /// <summary>
        /// Lists Roles. 
        /// </summary>
        /// <param name="max">Limit the maximum number of entries in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsListResult<RoleList> > ListRolesAsync(int? max = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("max", max?.ToString());

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsListResult<RoleList>, RoleList>(
                                    HttpMethod.Get,
                                    TEAMS_ROLE_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Gets role detail.
        /// </summary>
        /// <param name="roleId">Role id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Role> > GetRoleAsync(string roleId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<Role>, Role>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", TEAMS_ROLE_API_PATH, Uri.EscapeDataString(roleId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets role detail.
        /// </summary>
        /// <param name="role"><see cref="Role"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<Role> > GetRoleAsync(Role role, CancellationToken? cancellationToken = null)
        {
            return (GetRoleAsync(role.Id, cancellationToken));
        }

        #endregion


        #region Event APIs

        /// <summary>
        /// Lists Events. 
        /// </summary>
        /// <param name="resource">Limit results to a specific resource type.</param>
        /// <param name="type">Limit results to a specific event type.</param>
        /// <param name="actorId">Limit results to events performed by this person, by ID.</param>
        /// <param name="from">Limit results to events which occurred after a date and time.</param>
        /// <param name="to">Limit results to events which occurred before a date and time.</param>
        /// <param name="max">Limit the maximum number of entries in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsListResult<EventDataList> > ListEventsAsync(EventResource resource = null, EventType type = null, string actorId = null, DateTime? from = null, DateTime? to = null, int? max = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("resource", resource?.Name);
            queryParameters.Add("type",     type?.Name);
            queryParameters.Add("actorId",  actorId);
            queryParameters.Add("from",     from?.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz"));
            queryParameters.Add("to",       to?.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz"));
            queryParameters.Add("max",      max?.ToString());

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsListResult<EventDataList>, EventDataList>(
                                    HttpMethod.Get,
                                    TEAMS_EVENT_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Gets event detail.
        /// </summary>
        /// <param name="eventId">Event id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<EventData> > GetEventAsync(string eventId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<EventData>, EventData>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", TEAMS_EVENT_API_PATH, Uri.EscapeDataString(eventId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets event detail.
        /// </summary>
        /// <param name="eventData"><see cref="EventData"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<EventData> > GetEventAsync(EventData eventData, CancellationToken? cancellationToken = null)
        {
            return (GetEventAsync(eventData.Id, cancellationToken));
        }

        #endregion


        #region Resource Group APIs

        /// <summary>
        /// Lists Resource Groups. 
        /// </summary>
        /// <param name="organizationId">Specify the organization.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsListResult<ResourceGroupList> > ListResourceGroupsAsync(string organizationId = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("orgId", organizationId);

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsListResult<ResourceGroupList>, ResourceGroupList>(
                                    HttpMethod.Get,
                                    TEAMS_RESOURCE_GROUP_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Lists Resource Groups. 
        /// </summary>
        /// <param name="organization">Specify the <see cref="Organization"/>.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsListResult<ResourceGroupList> > ListResourceGroupsAsync(Organization organization, CancellationToken? cancellationToken = null)
        {
            return (ListResourceGroupsAsync(organization.Id, cancellationToken));
        }


        /// <summary>
        /// Gets Resource Group detail.
        /// </summary>
        /// <param name="resourceGroupId">Resource Group id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<ResourceGroup> > GetResourceGroupAsync(string resourceGroupId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<ResourceGroup>, ResourceGroup>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", TEAMS_RESOURCE_GROUP_API_PATH, Uri.EscapeDataString(resourceGroupId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets Resource Group detail.
        /// </summary>
        /// <param name="resourceGroup"><see cref="ResourceGroup"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<ResourceGroup> > GetResourceGroupAsync(ResourceGroup resourceGroup, CancellationToken? cancellationToken = null)
        {
            return (GetResourceGroupAsync(resourceGroup.Id, cancellationToken));
        }

        #endregion


        #region Resource Group Membership APIs


        /// <summary>
        /// Lists Resource Group Memberships. 
        /// </summary>
        /// <param name="licenseId">List resource group memberships for a license, by ID.</param>
        /// <param name="personId">List resource group memberships for a person, by ID.</param>
        /// <param name="personOrganizationId">List resource group memberships for an organization, by ID.</param>
        /// <param name="status">Limit resource group memberships to a specific status.</param>
        /// <param name="max">Limit the maximum number of items in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsListResult<ResourceGroupMembershipList> > ListResourceGroupMembershipsAsync(string licenseId = null, string personId = null, string personOrganizationId = null, ResourceGroupMembershipStatus status = null, int? max = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("licenseId",   licenseId);
            queryParameters.Add("personId",    personId);
            queryParameters.Add("personOrgId", personOrganizationId);
            queryParameters.Add("status",      status?.Name);
            queryParameters.Add("max",         max?.ToString());

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsListResult<ResourceGroupMembershipList>, ResourceGroupMembershipList>(
                                    HttpMethod.Get,
                                    TEAMS_RESOURCE_GROUP_MEMBERSHIP_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Lists Resource Group Memberships. 
        /// </summary>
        /// <param name="license">List resource group memberships for a license.</param>
        /// <param name="person">List resource group memberships for a person.</param>
        /// <param name="personOrganization">List resource group memberships for an organization.</param>
        /// <param name="status">Limit resource group memberships to a specific status.</param>
        /// <param name="max">Limit the maximum number of items in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsListResult<ResourceGroupMembershipList> > ListResourceGroupMembershipsAsync(License license = null, Person person = null, Organization personOrganization = null, ResourceGroupMembershipStatus status = null, int? max = null, CancellationToken? cancellationToken = null)
        {
            return (ListResourceGroupMembershipsAsync(license?.Id, person?.Id, personOrganization?.Id, status, max, cancellationToken));
        }

        
        /// <summary>
        /// Gets Resource Group Membership detail.
        /// </summary>
        /// <param name="resourceGroupMembershipId">Resource Group Membership id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<ResourceGroupMembership> > GetResourceGroupMembershipAsync(string resourceGroupMembershipId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<ResourceGroupMembership>, ResourceGroupMembership>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", TEAMS_RESOURCE_GROUP_MEMBERSHIP_API_PATH, Uri.EscapeDataString(resourceGroupMembershipId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets Resource Group Membership detail.
        /// </summary>
        /// <param name="resourceGroupMembership"><see cref="ResourceGroupMembership"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<ResourceGroupMembership> > GetResourceGroupMembershipAsync(ResourceGroupMembership resourceGroupMembership, CancellationToken? cancellationToken = null)
        {
            return (GetResourceGroupMembershipAsync(resourceGroupMembership.Id, cancellationToken));
        }


        /// <summary>
        /// Updates Resource Group membership.
        /// </summary>
        /// <param name="resourceGroupMembershipId">The ID of the resource group membership.</param>
        /// <param name="resourceGroupId">The ID of the resource group to which this resource group membership belongs.</param>
        /// <param name="licenseId">The license ID for the Hybrid Services licensed feature.</param>
        /// <param name="personId">The ID of the person to which this resource group membership is associated.</param>
        /// <param name="personOrganizationId">The OrganizationId for the person.</param>
        /// <param name="status">personOrganizationId</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task<TeamsResult<ResourceGroupMembership>> UpdateResourceGroupMembershipAsync(string resourceGroupMembershipId, string resourceGroupId, string licenseId, string personId, string personOrganizationId, ResourceGroupMembershipStatus status = null, CancellationToken? cancellationToken = null)
        {
            var membership = new ResourceGroupMembership
            {
                ResourceGroupId      = resourceGroupId,
                LicenseId            = licenseId,
                PersonId             = personId,
                PersonOrganizationId = personOrganizationId,
                StatusName           = status?.Name,
            };

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<ResourceGroupMembership>, ResourceGroupMembership>(
                                    HttpMethod.Put,
                                    new Uri(String.Format("{0}/{1}", TEAMS_RESOURCE_GROUP_MEMBERSHIP_API_PATH, Uri.EscapeDataString(resourceGroupMembershipId))),
                                    null,
                                    membership,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Updates Resource Group membership.
        /// </summary>
        /// <param name="resourceGroupMembership"><see cref="ResourceGroupMembership"/> to be updated.</param>
        /// <param name="resourceGroupId">The ID of the resource group to which this resource group membership belongs.</param>
        /// <param name="licenseId">The license ID for the Hybrid Services licensed feature.</param>
        /// <param name="personId">The ID of the person to which this resource group membership is associated.</param>
        /// <param name="personOrganizationId">The OrganizationId for the person.</param>
        /// <param name="status">The activation status of the resource group membership.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<ResourceGroupMembership> > UpdateResourceGroupMembershipAsync(ResourceGroupMembership resourceGroupMembership, string resourceGroupId, string licenseId, string personId, string personOrganizationId, ResourceGroupMembershipStatus status = null, CancellationToken? cancellationToken = null)
        {
            return (UpdateResourceGroupMembershipAsync(resourceGroupMembership.Id, resourceGroupId, licenseId, personId, personOrganizationId, status, cancellationToken));
        }

        /// <summary>
        /// Updates Resource Group membership.
        /// </summary>
        /// <param name="resourceGroupMembership"><see cref="ResourceGroupMembership"/> to be updated.</param>
        /// <param name="resourceGroup"><see cref="ResourceGroup"/> to which this resource group membership belongs.</param>
        /// <param name="license"><see cref="License"/> for the Hybrid Services licensed feature.</param>
        /// <param name="person"><see cref="Person"/> to which this resource group membership is associated.</param>
        /// <param name="personOrganization"><see cref="Organization"/> for the person.</param>
        /// <param name="status">The activation status of the resource group membership.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<ResourceGroupMembership> > UpdateResourceGroupMembershipAsync(ResourceGroupMembership resourceGroupMembership, ResourceGroup resourceGroup, License license, Person person, Organization personOrganization, ResourceGroupMembershipStatus status = null, CancellationToken? cancellationToken = null)
        {
            return (UpdateResourceGroupMembershipAsync(resourceGroupMembership.Id, resourceGroup.Id, license.Id, person.Id, personOrganization.Id, status, cancellationToken));
        }


        #endregion


    }

}
