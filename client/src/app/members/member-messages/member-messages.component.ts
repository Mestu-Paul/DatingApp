import { AfterViewChecked, AfterViewInit, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';
import { CommonModule } from '@angular/common';
import { TimeagoModule } from 'ngx-timeago';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css',
  imports: [CommonModule, TimeagoModule, FormsModule]
})
export class MemberMessagesComponent implements OnInit, AfterViewChecked, AfterViewInit{
  @ViewChild('messageForm') messageForm?: NgForm
  @Input() username?: string;
  messageContent:string = '';
  @ViewChild('messageList') messageList!: ElementRef;

  shouldScrollDown=false;

  constructor(public messageService: MessageService) {}
  ngAfterViewInit(): void {
    this.shouldScrollDown=true;
  }
  ngAfterViewChecked(): void {
    if(this.shouldScrollDown){
      try{
        this.messageList.nativeElement.scrollTop = this.messageList.nativeElement.scrollHeight;
      }catch(error ){
        // console.log(error);
      }

    }
  }

  ngOnInit(): void {
    
  }

  sendMessage(){
    if(!this.username)return;
    this.messageService.sendMessage(this.username,this.messageContent).then(() => {
      this.messageForm?.reset();
      this.messageList.nativeElement.scrollTop = this.messageList.nativeElement.scrollHeight;
    })
  }

  

}
