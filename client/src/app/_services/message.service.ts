import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { Message } from '../_models/message';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { Group } from '../_models/group';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient) { }

  createHubConnection(user: User, otherUsername: string){
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl+'message?user='+otherUsername, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();
    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on("ReceivedMessageThread", messages => {
      this.messageThreadSource.next(messages);
      console.log("Rece");
    });
    
    this.hubConnection.on("NewMessage", message => {
      this.messageThread$.pipe(take(1)).subscribe({
        next: messages => {
          this.messageThreadSource.next([...messages,message]);
        }
      })
    });
    
    this.hubConnection.on("UpdatedGroup", (group: Group) => {
      if(group.connection.some(x => x.username === otherUsername)){
        this.messageThread$.pipe(take(1)).subscribe({
          next: messages => {
            messages.forEach(message => {
              if(!message.dateRead){
                message.dateRead = new Date(Date.now());
              }
            })
            this.messageThreadSource.next([...messages]);
          }
        })
      }
    });
  }

  stopHubConnection(){
    if(this.hubConnection){
      this.hubConnection.stop().catch(error => console.log(error));
    }

  }

  getMessages(pageNumber: number, pageSize: number, container: string){
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    return getPaginatedResult<Message[]>(this.baseUrl+'messages', params, this.http);
  }

  getMessageThread(username: string){
    return this.http.get<Message[]>(this.baseUrl+'messages/thread/'+username);
  }

  async sendMessage(username: string, content: string){
    return this.hubConnection?.invoke('SendMessage', {recipientUsername: username, content})
      .catch(error => console.error("Error invoking SendMessage:",error));
  }

  deleteMessage(id: Number){
    return this.http.delete(this.baseUrl+'messages/'+id);
  }
}