import { Component, OnInit } from '@angular/core';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { Pagination } from '../../_models/pagination';
import { UserParams } from '../../_models/userParams';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css'
})
export class MemberListComponent implements OnInit{
  members: Member[] = [];
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  genderList = [{value: 'male', display: 'Male'}, {value: 'female', display: 'Female'}]
  
  constructor(private memberService: MembersService ) {
    this.memberService.initUserParams();
    this.userParams = this.memberService.getUserParams();
  }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers(){
    if(!this.userParams)return;
    this.memberService.setUserParams(this.userParams);
    this.memberService.getMembers(this.userParams).subscribe({
      next: response => {
        if(response.result && response.pagination){
          this.members = response.result;
          this.pagination  =response.pagination;
        }
      }
    })
  }

  resetFilters(){
    this.userParams =  this.memberService.resetUserParams();
    this.loadMembers();
  }

  pageChanged(event: any){
    if(!this.userParams)return;
    if(this.userParams.pageNumber!==event.page){
      this.userParams.pageNumber = event.page;
      this.memberService.setUserParams(this.userParams);
      this.loadMembers();
    }
  }
}
