using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RumpoleGateway.Clients.UserClient
{
    public class UserClient : IUserClient
    {
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        //private readonly IGraphServiceClientFactory _graphServiceClientFactory;
        //private readonly IGraphServiceClientWrapper _graphServiceClientWrapper;
        //private readonly IUserGroupMapper _userGroupMapper;

        public UserClient(
            IOnBehalfOfTokenClient onBehalfOfTokenClient
            //IGraphServiceClientFactory graphServiceClientFactory,
            //IGraphServiceClientWrapper graphServiceClientWrapper,
            //IUserGroupMapper userGroupMapper
            )
        {
            _onBehalfOfTokenClient = onBehalfOfTokenClient;
            //_graphServiceClientFactory = graphServiceClientFactory;
            //_graphServiceClientWrapper = graphServiceClientWrapper;
            //_userGroupMapper = userGroupMapper;
        }

        public async Task<Domain.Authorisation.User> GetUser(string accessToken)
        {
            var onBehalfOfAccessToken = await _onBehalfOfTokenClient.GetAccessToken(accessToken);

            //var graphClient = _graphServiceClientFactory.CreateGraphServiceClient(onBehalfOfAccessToken);

            //var (displayName, userPrincipalName) = await _graphServiceClientWrapper.GetUserNameAsync(graphClient);
            //var groups = await _graphServiceClientWrapper.GetUserGroupsAsync(graphClient);

            //var (roles, areaDivisions) = _userGroupMapper.GetRolesAndAreaDivisions(groups.ToList());

            return new Domain.Authorisation.User
            {
                DisplayName = "displayName",
                UserPrincipalName = "userPrincipalName",
                //Roles = roles,
                //AreaDivisions = areaDivisions
            };
        }
    }
}
