import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { Observable, map } from 'rxjs';

export const authGuard: CanActivateFn = (route, state):Observable<boolean>|boolean => {

  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);

  return accountService.currentUser$.pipe(
    map(user => {
      if(user){
        console.log("Calling me valid user",user);
        return true;
      }
      else{
        console.log("Calling me invalid user",user);
        toastr.error('you shall not pass!');
        return false;
      }
    })
  )
};