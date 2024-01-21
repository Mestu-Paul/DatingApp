import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;
  
  constructor(private http:HttpClient) { }

  getMembers(){
    return this.http.get<Member[]>(this.baseUrl+'users')
  }

  getMember(username:string){
    return this.http.get<Member>(this.baseUrl+'users/'+username);
  }

  updateMember(member: Member){
    return this.http.put(this.baseUrl+'users',member);
  }

  uploadPhoto(image: FormData){
    return this.http.post(this.baseUrl+'users/add-photo',image);
  }

  setMainPhoto(photoId: number){
    return this.http.put(this.baseUrl + 'users/set-main-photo/'+ photoId, {});
  }

  deletePhoto(photoId: number){
    return this.http.delete(this.baseUrl+'users/delete-photo/'+photoId);
  }
}
