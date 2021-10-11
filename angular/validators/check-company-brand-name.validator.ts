import { Injectable } from '@angular/core';
import { AsyncValidator, AbstractControl, ValidationErrors } from '@angular/forms';
import { UserService } from '../services';
import { catchError, map } from 'rxjs/operators';
import { Observable, of } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class CheckCompanyAndBrandNameValidator implements AsyncValidator {
  constructor(private _userService: UserService) {}

  validate(ctrl: AbstractControl): Promise<ValidationErrors | null> | Observable<ValidationErrors | null> {
      if(ctrl.value == undefined || ctrl.value == ''){
          return of(null);
      }
    return this._userService.checkCompanyAndBrandName(ctrl.value).pipe(
      map((isUnique: boolean) => 
            (!isUnique ? { 'checkCompanyName': true } : null)
        ),
      catchError(() => of(null))
    );
  }
}