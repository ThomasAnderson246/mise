export const INCOMPLETE_REASONS = [
    {code: 'missing_ingredient', label: 'Missing Ingredient', noteRequired: true},
    {code: 'waiting_on_prep', label: 'Waiting on another prep item', noteRequired: true},
    {code: 'equipment_unavailable', label: 'Equipment Unavailable', noteRequired: true},
    {code: 'ran_out_of_time', label: 'Ran out of time', noteRequired: false},
    {code: 'other', label: 'Other', noteRequired: true},
]

export interface IncompleteReasonValue{
    reasonCode: string
    reasonNote: string
}

export const EMPTY_REASON: IncompleteReasonValue = {reasonCode:'', reasonNote: ''}

export function getReasonLabel(code: string | null): string {
    return INCOMPLETE_REASONS.find((r) => r.code === code)?.label ?? code ?? ''
}

export function isReasonComplete(value: IncompleteReasonValue): boolean {
    const reason = INCOMPLETE_REASONS.find((r) => r.code === value.reasonCode)
    if (!reason) return false
    return !reason.noteRequired || value.reasonNote.trim().length > 0
}