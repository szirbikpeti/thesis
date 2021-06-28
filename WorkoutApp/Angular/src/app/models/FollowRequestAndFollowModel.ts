export interface FollowRequestAndFollowModel {
  sourceUsers: FollowRequestModel[],
  targetUsers: FollowRequestModel[],
  followerUserIds: string[],
  followedUserIds: string[],
}

interface FollowRequestModel {
  id: string,
  isBlocked: boolean
}
