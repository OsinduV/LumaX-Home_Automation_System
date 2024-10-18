import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AuthService } from '../../shared/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './registration.component.html'
})
export class RegistrationComponent {
  form!: FormGroup;
  isSubmitted:boolean = false;

  constructor(
    private formBuilder: FormBuilder,
    private service: AuthService,
    private toastr:ToastrService
  ){}
  
  ngOnInit(): void{
    this.form = this.formBuilder.group({
      fullName : ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],  
      password : ['', [
        Validators.required,
        Validators.minLength(6),
        Validators.pattern(/^(?=.*[A-Za-z0-9])(?=.*[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?])/),
      ]],
      confirmPassword : [''],
    }, {validators:this.passwordMatchValidator})
    
  }
  
  passwordMatchValidator: ValidatorFn = (control:AbstractControl):null =>{
    const password = control.get('password')
    const confirmPassword = control.get('confirmPassword')

    if(password && confirmPassword && password.value != confirmPassword.value)
      confirmPassword?.setErrors({passwordMismatch:true})
    else
      confirmPassword?.setErrors(null)

    return null;
  }
  onSubmit(){
    this.isSubmitted = true;
    if(this.form.valid){
      console.log(this.form.value);
      this.service.createUser(this.form.value)
      .subscribe({
        next : (res:any) => {
          if(res.succeeded){
            this.form.reset();
            this.isSubmitted = false;
            this.toastr.success('New User Created!', 'Registration Successfull')
          }  
        },
        error : err => {
          if(err.error.errors){
            err.error.errors.forEach((x:any)=>{
              switch(x.code){
                case "DuplicateUserName":
                  break;

                case "DuplicateEmail":
                  this.toastr.error('Email is Already Taken.', 'Registration Failed');
                  break;

                  default:
                    this.toastr.error('Contact the developer', 'Registration Failed');
                    console.log(x);
                    break;
              }
            })
          }else
            console.log('error:', err)

        }
      });
    }
  }

  hasDisplayableError(controlName: string):Boolean{
    const control = this.form.get(controlName);
    return Boolean(control?.invalid) && 
    (this.isSubmitted || Boolean(control?.touched))
  }

}
