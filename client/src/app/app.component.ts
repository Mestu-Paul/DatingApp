import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'Dating App';

  constructor(private accountService: AccountService){

  }
  ngOnInit(): void {
    this.setCurrentUser();
  }


  setCurrentUser(){
    var userString = null;
    try {
      userString = localStorage.getItem('user');
    } catch (error) {
      console.log("local error ", error);
    }
    if(!userString)return;
    const user:User = JSON.parse(userString);
    this.accountService.setCurrentUser(user);
  }
}
