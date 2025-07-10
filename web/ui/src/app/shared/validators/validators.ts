import { FormArray, FormGroup, ValidationErrors, ValidatorFn } from '@angular/forms';
import * as _ from 'lodash';

// 用在檢查FormGroup 若有填寫一格，則全部都要填寫
export function formAtLeastRequiredCheckValidator(): ValidatorFn {
    return (form: FormGroup): ValidationErrors | null => {
        let valueList = [];
        Object.keys(form.controls).forEach((each: string) => {
            if (!_.isEmpty(form.get(each).value)) {
                valueList.push(each);
            }
        });

        if (valueList.length === 0) {
          return null;
        }

        if (valueList.length !== Object.keys(form.controls).length) {
          return { ['requiredError']: true };
        }

        return null;
    };
}
