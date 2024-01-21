import { Component, Input, OnInit } from '@angular/core';
import { Member } from '../../_models/member';
import { User } from '../../_models/user';
import { AccountService } from '../../_services/account.service';
import { take } from 'rxjs';
import { MembersService } from '../../_services/members.service';
import { Photo } from '../../_models/photo';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrl: './photo-editor.component.css'
})
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member | undefined;
  imageFile: File | null = null;
  user: User | undefined;
  
  constructor(private accountService: AccountService, private memberService: MembersService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if(user) this.user = user;
      }
    })
  }


  ngOnInit(): void {
    
  }

  selectImage(event: any){
    this.imageFile = event.target.files[0];
  }

  uploadImage(){
    if(this.imageFile){
      const formData: FormData = new FormData();
      formData.append('file', this.imageFile);
      this.memberService.uploadPhoto(formData).subscribe({
        next: response => {
          if(response){
            const photo = response as Photo;
            this.member?.photos.push(photo);
            if(photo.isMain && this.user && this.member){
              this.user.photoUrl = photo.url;
              this.member.photoUrl = photo.url;
              this.accountService.setCurrentUser(this.user);
            }
          }

        },
        error: error => console.log(error)
      });
    }
    else{
      alert('You are not select any image');
    }
  }

  setMainPhoto(photo: Photo){
    this.memberService.setMainPhoto(photo.id).subscribe({
      next: () => {
        if (this.user && this.member){
          this.user.photoUrl = photo.url;
          this.accountService.setCurrentUser(this.user);
          this.member.photoUrl = photo.url;
          this.member.photos.forEach( p => {
            if(p.isMain)p.isMain = false;
            if(p.id==photo.id)p.isMain = true;
          })
        }
      }
    });
  }

  deletePhoto(photoId: number){
    this.memberService.deletePhoto(photoId).subscribe({
      next: () => {
        if(this.member){
          this.member.photos = this.member.photos.filter(x => x.id !== photoId);
        }
      }
    })
  }
}
