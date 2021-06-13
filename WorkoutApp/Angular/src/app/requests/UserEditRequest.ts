import {PasswordChangeRequest} from "./PasswordChangeRequest";

export interface UserEditRequest {
  fullName: string,
  email: string,
  about: string,
  profilePictureId: string,
  birthday: Date,
  passwordChange: PasswordChangeRequest
}
