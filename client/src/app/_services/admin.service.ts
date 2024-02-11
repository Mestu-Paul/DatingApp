import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { environment } from '../../environments/environment';
import { Photo } from '../_models/photo';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getUserWithRoles(){
    return this.http.get<any[]>(this.baseUrl+'admin/users-with-roles');
  }

  updateUserRoles(username:string, roles: string[]){
    return this.http.post<string[]>(this.baseUrl+'admin/edit-roles/'+username+'?roles='+roles,{});
  }

  getUnapprovedPhotos(){
    return this.http.get<Photo[]>(this.baseUrl+'admin/photos-to-moderate');
  }

  approvePhoto(photoId:number){
    return this.http.put(this.baseUrl+'admin/photos-to-moderate/approve/'+photoId,{});
  }
  
  deletePhoto(photoId:number){
    return this.http.delete(this.baseUrl+'admin/photos-to-moderate/delete/'+photoId);
  }
}
