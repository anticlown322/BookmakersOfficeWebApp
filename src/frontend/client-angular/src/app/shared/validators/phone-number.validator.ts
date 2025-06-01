import { AbstractControl, ValidationErrors } from "@angular/forms";

  export function phoneNumberValidator(control: AbstractControl): ValidationErrors | null {
    if (!control.value) return null;
    const phoneRegex = /^\+?[0-9]{10,15}$/;
    return phoneRegex.test(control.value) ? null : { invalidPhone: true };
  }