export const ACTION_GROUPS: Record<string, string[]> = {
    create: ['create', 'invite', 'create_draft', 'add_ingredient', 'add_step', 'add_item', 'add_recipe', 'add_sub_recipe'],
    update: ['update', 'update_ingredient', 'save_draft', 'assign', 'assign_permission', 'resolve_allergens'],
    delete: ['delete', 'remove_ingredient', 'remove_item', 'remove_recipe', 'discard_draft'],
    publish: ['publish'],
    complete: ['complete_item', 'force_complete'],
    user: ['deactivate', 'reactivate']
}

export const ENTITY_TYPES = [
    'recipe',
    'ingredient',
    'user',
    'role',
    'prep_list',
    'menu_item',
    'allergen'
]

export const DATE_RANGES = [
    { label: 'today', value: 'today'},
    { label: 'Last 7 days', value: '7days'},
    { label: 'Last 30 days', value: '30days'},
    { label: 'All time', value: 'all'}
]

export function getActionGroup(action: string): string {
    for (const [group, actions] of Object.entries(ACTION_GROUPS)) {
        if (actions.includes(action)) return group
    }

    return 'other'
}

export function getHumanReadable(performedByName: string | null, action: string, resource: string): string {
    const who = performedByName ?? 'System'
    const what = resource

    switch (action) {
        case 'create': return `${who} created a ${what}`
        case 'update': return `${who} updated a ${what}`
        case 'delete': return `${who} deleted a ${what}`
        case 'publish': return `${who} published a ${what}`
        case 'invite': return `${who} invited a new user`
        case 'deactivate': return `${who} deactivated a user`
        case 'reactivate': return `${who} reactivated a user`
        case 'create_draft': return `${who} created a draft ${what}`
        case 'save_draft': return `${who} saved a draft ${what}`
        case 'discard_draft': return `${who} discarded a draft ${what}`
        case 'add_ingredient': return `${who} added an ingredient to a ${what}`
        case 'remove_ingredient': return `${who} removed an ingredient from a ${what}`
        case 'add_step': return `${who} added a step to a ${what}`
        case 'add_item': return `${who} added an item to a prep list`
        case 'remove_item': return `${who} removed an item from a prep list`
        case 'add_recipe': return `${who} added a recipe to a ${what}`
        case 'remove_recipe': return `${who} removed a recipe from a ${what}`
        case 'add_sub_recipe': return `${who} linked a sub-recipe`
        case 'assign': return `${who} assigned a ${what}`
        case 'assign_permission': return `${who} assigned a permission to a role`
        case 'complete_item': return `${who} completed a prep list item`
        case 'force_complete': return `${who} force completed a prep list`
        case 'resolve_allergens': return `${who} resolved allergens on a menu item`
        case 'update_ingredient': return `${who} updated an ingredient`
        default: return `${who} performed ${action} on ${what}`
    }
}

export function formatRelativeTime(dateStr: string): string {
    const date = new Date(dateStr)
    const now = new Date()
    const diffMs = now.getTime() - date.getTime()
    const diffMins = Math.floor(diffMs / 60000)
    const diffHours = Math.floor(diffMins / 60)
    const diffDays = Math.floor(diffHours / 24)

    if (diffMins < 1) return 'just now'
    if (diffMins < 60) return `${diffMins}m ago`
    if (diffHours < 24) return `${diffHours}h ago`
    if (diffDays < 7) return `${diffDays}d ago`
    return date.toLocaleDateString()
}

export function getFromDate(range: string): Date | null {
    const now = new Date()
    switch (range) {
        case 'today':
            return new Date(now.getFullYear(), now.getMonth(), now.getDate())
        case '7days':
            return new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000)
        case '30days':
            return new Date(now.getTime() - 30 * 24 * 60 * 60 * 1000)
        default: 
            return null
    }
}

export function getActionGroupColor(group: string): string {
    switch (group){
        case 'delete': return 'bg-red-100 text-red-800'
        case 'publish': return 'bg-green-100 text-green-800'
        case 'user': return 'bg-purple-100 text-purple-800'
        default: return 'bg-muted text-muted-foreground'
    }
}