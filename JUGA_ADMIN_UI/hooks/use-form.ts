// app/_hooks/use-form.ts
import { useState, useCallback } from "react";
import { toast } from "sonner";

// --- Type Definitions ---
// Describes the shape of the validation errors object.
type FormErrors<T> = Partial<Record<keyof T, string>>;
// Describes the shape of the touched fields object.
type FormTouched<T> = Partial<Record<keyof T, boolean>>;

// --- Hook Props ---
interface UseFormProps<T> {
  // The initial values for the form fields.
  initialValues: T;
  // A function that receives the current form values and returns an object with any validation errors.
  validate: (values: T) => FormErrors<T>;
  // The callback to execute when the form is submitted and passes validation.
  onSubmit: (values: T) => void;
}

/**
 * A custom hook to manage form state, validation, and submission.
 * @param initialValues - The starting values for the form.
 * @param validate - A function to validate the form's values.
 * @param onSubmit - The function to call on successful submission.
 */
export function useForm<T>({ initialValues, validate, onSubmit }: UseFormProps<T>) {
  const [values, setValues] = useState<T>(initialValues);
  const [errors, setErrors] = useState<FormErrors<T>>({});
  const [touched, setTouched] = useState<FormTouched<T>>({});

  /**
   * Handles changes to form inputs.
   * It updates the value and re-validates the field if it has been touched before.
   */
  const handleChange = useCallback(
    (field: keyof T, value: any) => {
      setValues((prev) => ({ ...prev, [field]: value }));

      if (touched[field]) {
        const validationErrors = validate({ ...values, [field]: value });
        setErrors((prev) => ({ ...prev, [field]: validationErrors[field] }));
      }
    },
    [touched, values, validate]
  );

  /**
   * Handles the blur event on form inputs.
   * It marks the field as touched and triggers validation for that field.
   */
  const handleBlur = useCallback(
    (field: keyof T) => {
      setTouched((prev) => ({ ...prev, [field]: true }));
      const validationErrors = validate(values);
      setErrors((prev) => ({ ...prev, [field]: validationErrors[field] }));
    },
    [values, validate]
  );

  /**
   * Handles the form submission process.
   * It validates all fields, displays errors if any, and calls the onSubmit callback if valid.
   */
  const handleSubmit = useCallback(
    (e?: React.FormEvent<HTMLFormElement>) => {
      e?.preventDefault(); // Prevent default form submission

      const validationErrors = validate(values);
      setErrors(validationErrors);

      // Mark all fields as touched to display errors for un-touched fields
      const allTouched = Object.keys(initialValues as object).reduce((acc, key) => {
        acc[key as keyof T] = true;
        return acc;
      }, {} as FormTouched<T>);
      setTouched(allTouched);

      // If there are no errors, call the provided onSubmit function
      if (Object.keys(validationErrors).length === 0) {
        onSubmit(values);
      } else {
        toast.error("Lütfen tüm zorunlu alanları doğru şekilde doldurun");
      }
    },
    [values, validate, onSubmit, initialValues]
  );

  return {
    values,
    setValues, // Expose setValues for complex imperative updates (like auto-generating identifier)
    errors,
    touched,
    handleChange,
    handleBlur,
    handleSubmit,
  };
}
