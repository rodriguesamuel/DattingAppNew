import { Component, OnInit } from '@angular/core';
import { Photo } from '../../_models/photo';
import { AdminService } from '../../_services/admin.service';
import { ConfirmService } from '../../_services/confirm.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css'],
})
export class PhotoManagementComponent implements OnInit {
  photos: Partial<Photo[]>;
  constructor(
    private adminService: AdminService,
    private confirmService: ConfirmService
  ) {}

  ngOnInit(): void {
    this.getPhotosForApproval();
  }

  getPhotosForApproval() {
    this.adminService.getPhotoForApproval().subscribe((photos) => {
      this.photos = photos;
    });
  }

  aprovePhoto(photoid: number) {
    this.confirmService
      .confirm('Confirm aproval of this photo', 'This cannot be undone')
      .subscribe((result) => {
        if (result) {
          this.adminService.approvePhoto(photoid).subscribe(() => {
            this.photos.splice(
              this.photos.findIndex((p) => p.id === photoid),
              1
            );
          });
        }
      });
  }

  rejectPhoto(photoid: number) {
    this.confirmService
      .confirm('Confirm rejection of this photo', 'This cannot be undone')
      .subscribe((result) => {
        if (result) {
          this.adminService.rejectPhoto(photoid).subscribe(() => {
            this.photos.splice(
              this.photos.findIndex((p) => p.id === photoid),
              1
            );
          });
        }
      });
  }
}
