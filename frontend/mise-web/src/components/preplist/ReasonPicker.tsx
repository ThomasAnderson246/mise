import { INCOMPLETE_REASONS } from "@/config/incompleteReasons";
import type { IncompleteReasonValue } from "@/config/incompleteReasons";
import { inputClass, selectClass } from "@/lib/styles";

interface ReasonPickerProps {
  value: IncompleteReasonValue;
  onChange: (value: IncompleteReasonValue) => void;
}

export function ReasonPicker({ value, onChange }: ReasonPickerProps) {
  const selected = INCOMPLETE_REASONS.find((r) => r.code === value.reasonCode);

  return (
    <div className="space-y-2">
      <select
        value={value.reasonCode}
        onChange={(e) => onChange({ ...value, reasonCode: e.target.value })}
        className={selectClass}
      >
        <option value="">Select a reason...</option>
        {INCOMPLETE_REASONS.map((r) => (
          <option key={r.code} value={r.code}>
            {r.label}
          </option>
        ))}
      </select>
      {selected && (
        <input
          type="text"
          value={value.reasonNote}
          onChange={(e) => onChange({ ...value, reasonNote: e.target.value })}
          placeholder={
            selected.noteRequired
              ? "What's going on? (required)"
              : "Add a note (optional)"
          }
          className={inputClass}
        />
      )}
    </div>
  );
}
