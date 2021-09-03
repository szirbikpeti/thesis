import {Component, Inject, OnDestroy} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {MessageService} from "../../services/message.service";
import {UserModel} from "../../models/UserModel";
import {MessageModel} from "../../models/MessageModel";
import {MessageRequest} from "../../requests/MessageRequest";
import {UserService} from "../../services/user.service";
import {StateService} from "../../services/state.service";
import {getPicture} from "../../utility";
import {DomSanitizer} from "@angular/platform-browser";
import * as signalR from "@microsoft/signalr";

@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.scss']
})
export class MessageComponent implements  OnDestroy {

  currentUser: UserModel;

  users: UserModel[];
  messages: MessageModel[];

  triggeredUserId: string;

  getPicture = getPicture;

  connection: signalR.HubConnection;

  constructor(private _message: MessageService, private _user: UserService, private _state: StateService,
              private route: ActivatedRoute, private router: Router, public sanitizer: DomSanitizer,
              @Inject('BASE_URL') baseUrl: string) {
    this.currentUser = _state.user.value;

    this.route.paramMap.subscribe( paramMap => {
      this.triggeredUserId = paramMap.get('id');
      this.getMessages();
    });

    _message.listUsersWithMessage().subscribe(users => {
      this.users = users;

      if (users.length === 0) {
        this.messages = [];
        return;
      }

      if (!this.triggeredUserId) {
        router.navigate([users[0].id], {relativeTo: route});
        return;
      }

      this.getMessages();
    });

    this.connection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Information)
      .withUrl(baseUrl + "notify-message")
      .build();

    this.connection.start()
      .then(null)
      .catch(err => console.error(err.toString()));

    this.connection.on("BroadcastMessage", () => this.getMessages());
  }

  sendMessage(messageInput: HTMLInputElement): void {
    const request: MessageRequest = { message: messageInput.value, triggeredUserId: this.triggeredUserId };

    this._message.send(request).subscribe(message => {
      messageInput.value = '';
      this.messages.push(message);

      setTimeout(() => {
        const messagesContainer = document.getElementById("messages");
        messagesContainer.scrollTop = messagesContainer?.scrollHeight;
      }, 100);
    });
  }

  isCurrentUserMessage(senderUserId: string): boolean {
    return senderUserId === this.currentUser.id;
  }

  switchUser(userId: string): void {
    this.messages = null;
    this.router.navigate(['messages', userId]);
  }

  private getMessages(): void {
    if (!this.triggeredUserId) {
      return;
    }

    this._message.listMessages(this.triggeredUserId).subscribe(messages => {
      this.messages = messages;

      if (messages.length === 0) {
        this._user.getById(this.triggeredUserId).subscribe(user => {
          if (!this.users?.map(({id}) => id).includes(user.id)) {
            this.users?.unshift(user);
          }
        });
      }

      setTimeout(() => {
        const messagesContainer = document.getElementById("messages");
        messagesContainer.scrollTop = messagesContainer?.scrollHeight;
      }, 100);
    });
  }

  ngOnDestroy(): void {
    this.connection.stop().then(null);
  }
}
