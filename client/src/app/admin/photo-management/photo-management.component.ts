import { Component, OnInit } from '@angular/core';
import { Photo } from '../../_models/photo';
import { AdminService } from '../../_services/admin.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrl: './photo-management.component.css'
})
export class PhotoManagementComponent implements OnInit{
  photos: Photo[] = [];
  constructor(private adminService:AdminService, private toastr: ToastrService){

  }
  ngOnInit(): void {
    this.loadPhotos();
  }

  loadPhotos(){
    this.adminService.getUnapprovedPhotos().subscribe({
      next: photos => this.photos = photos,
      error: error => console.log(error)
    })
  }
  
  approvePhoto(photoId: number){
    this.adminService.approvePhoto(photoId).subscribe({
      next: res => {
        this.toastr.success("Approved");
        this.photos = this.photos.filter(photo => photo.id !== photoId);
      },
      error: error => this.toastr.error(error),
    })
  }

  deletePhoto(photoId: number){
    this.adminService.deletePhoto(photoId).subscribe({
      next: res => {
        this.toastr.success("Deleted");
        this.photos = this.photos.filter(photo => photo.id !== photoId);
      },
      error: error => this.toastr.error(error),
    })
  }
}
