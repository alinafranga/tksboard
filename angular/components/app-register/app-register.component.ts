import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, AbstractControl } from '@angular/forms';
import { CheckBrandNameValidator, UserModel, UserType } from '../../shared/';
import { UserService } from '../../shared/services';
import { CheckEmailValidator } from '../../shared/validators/check-email.validator';
import { Router } from '@angular/router';
import { PhoneMask } from '../../shared/masks';
import { CheckCompanyNameValidator } from '../../shared/validators/check-company-name.validator';
import { CheckCompanyAndBrandNameValidator } from '../../shared/validators/check-company-brand-name.validator';
import { combineLatest, of } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-register',
  templateUrl: './app-register.component.html',
  styleUrls: ['./app-register.component.css']
})
export class AppRegisterComponent implements OnInit {

  public registerForm: FormGroup;
  public producer: UserModel;
  public submitted = false;
  public errorServer = false;
  public goodServer = false;
  public showBar = false;

  constructor(public phoneMask: PhoneMask, private _userService: UserService, private _checkEmail: CheckEmailValidator, private _router: Router,
    public userType: UserType, private _checkBrandName: CheckBrandNameValidator, private _checkCompanyName: CheckCompanyNameValidator,
    private _checkCompanyAndBrand: CheckCompanyAndBrandNameValidator) {
    this.producer = new UserModel();
    this.registerForm = new FormGroup({
      userTypeId: new FormControl('', Validators.required),
      name: new FormControl('', Validators.required),
      phone: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required,
      Validators.pattern("^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$")],
        [this._checkEmail.validate.bind(this._checkEmail)]),
      company: new FormControl('', [Validators.required, Validators.maxLength(200)],),
      license: new FormControl('', Validators.required),
      brand: new FormControl('', Validators.maxLength(200), [this._checkBrandName.validate.bind(this._checkBrandName)])
    });
  }

  ngOnInit() {
    this.setBrandValidator();
  }

  setBrandValidator() {
    const companyControl = this.registerForm.get('company');
    this.registerForm.get('userTypeId').valueChanges
      .subscribe(userTypeIdSelected => {
        if (userTypeIdSelected == this.userType.Retailer) {
          companyControl.setAsyncValidators([this._checkCompanyAndBrand.validate.bind(this._checkCompanyAndBrand)]);
        } else {
          companyControl.setAsyncValidators([this._checkCompanyName.validate.bind(this._checkCompanyName)]);
        }
        companyControl.updateValueAndValidity();
      });
  }

  isFieldValid(field: string) {
    var valid = this.submitted && ((!this.registerForm.get(field).valid && this.registerForm.get(field).touched) ||
      (this.registerForm.get(field).untouched && (this.registerForm.get(field).value == '' ||
        this.registerForm.get(field).value == null || this.registerForm.get(field).value == undefined)));
    if (field == "phone" && this.registerForm.get('phone').value && this.registerForm.get('phone').value.length >= 10) {
      return false;
    }
    return valid;
  }

  checkIfFormPassesValidation(formGroup: FormGroup) {
    const syncValidationErrors = Object.keys(formGroup.controls).map(c => {
      const control = formGroup.controls[c];
      if (c != 'phone') {
        return !control.validator ? null : control.validator(control);
      }
    }).filter(errors => !!errors);
    return combineLatest(Object.keys(formGroup.controls).map(c => {
      const control = formGroup.controls[c];
      return !control.asyncValidator ? of(null) : control.asyncValidator(control)
    })).pipe(
      map(asyncValidationErrors => {
        const hasErrors = [...syncValidationErrors, ...asyncValidationErrors.filter(errors => !!errors)].length;
        if (hasErrors) { // ensure errors display in UI...
          Object.keys(formGroup.controls).forEach(key => {
            formGroup.controls[key].markAsTouched();
            formGroup.controls[key].updateValueAndValidity();
          })
        }
        return !hasErrors;
      })).toPromise();
  }

  isCompanyRequiredValid() {
    let isRequiredError = false;
    if (this.company.errors?.required) {
      isRequiredError = true;
    }
    let valid = this.submitted && ((isRequiredError && this.company.touched) ||
      (this.company.untouched && (this.company.value == '' ||
        this.company.value == null || this.company.value == undefined)));
    return valid;
  }
  public get brand() {
    return this.registerForm.get('brand');
  }

  public get company() {
    return this.registerForm.get('company');
  }

  onSubmit() {
    this.submitted = true;
    this.showBar = true;
    if (!this.registerForm.get('phone').valid) {
      if (this.registerForm.get('phone').errors['mask'] && this.registerForm.get('phone').value.length >= 10) {
        this.registerForm.get('phone').setErrors(null);
      } else {
        this.showBar = false;
        return;
      }
    }
    if (this.registerForm.valid) {
      this.producer.Active = true;
      this.producer.AccountStatusId = 2;
      this.producer.Id = 0;
      this.producer.UserTypeId = parseInt(this.registerForm.get('userTypeId').value);
      if (this.producer.UserTypeId == this.userType.Retailer && (this.producer.BrandName == undefined || this.producer.BrandName === '')) {
        this.producer.BrandName = this.producer.CompanyName;
      }
    }
    this.checkIfFormPassesValidation(this.registerForm)
      .then(valid => {
        if (valid) {
          this._userService.register(this.producer).subscribe((data) => {
            if (data > 0) {
              this.registerForm.reset();
              this._router.navigate(['/data/thank-you']);
            } else {
              this.errorServer = true;
              this.showBar = false;
            }
            this.submitted = false;
            this.showBar = false;
          }, (error) => {
            this.submitted = false;
            this.showBar = false;
          });
        } else {
          this.showBar = false;
        }
      });
  }
}
