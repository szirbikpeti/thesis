export enum NotificationType {
  FOLLOW_REQUEST = 0,
  DELETE_FOLLOW_REQUEST = 1,
  DELETE_DECLINED_FOLLOW_REQUEST = 2,
  FOLLOW_BACK = 3,
  ACCEPT_FOLLOW_REQUEST = 4,
  DECLINE_FOLLOW_REQUEST = 5,
  UNFOLLOW = 6,
  ADD_LIKE = 10,
}

export enum NotificationCategory {
  Follow,
  General,
  Message
}
