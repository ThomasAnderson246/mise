import {Link, useLocation, useParams } from 'react-router-dom'
import { useAuth } from '@/context/AuthContext'
import { cn } from '@/lib/utils'

interface NavItem{
    label: string
    path: string
    icon: string
    permission?: {resource:string; action:string}
}

const primaryNavItems: NavItem[] =[
    {label: 'dashboard', path: 'dashboard', icon:''},
    {label: 'Recipes', path: 'recipes', icon: '', permission: {resource:'recipe', action:'read'}},
    {label: 'Prep Lists', path: 'prep-lists', icon: '', permission: {resource:'preplist', action:'read'}},
    {label: 'Menu', path: 'menu-items', icon:'', permission: {resource: 'menuitem', action:'read'}},
    {label: 'More', path: 'more', icon: ''},
]

export function BottomNav(){
    const { slug } = useParams<{ slug: string }>()
    const { hasPermission } = useAuth()
    const location = useLocation()

    const visibleItems = primaryNavItems.filter(item =>
        !item.permission || hasPermission(item.permission.resource, item.permission.action)
    )

    return (
        <nav className="md:hidden fixed bottom-o left-0 right-0 bg-sidebar border-t border-sidebar-border z-50">
            <div className="flex items-center justify-around px-2 py-2">
                {visibleItems.map(item =>{
                    const fullPath = `/${slug}/${item.path}`
                    const isActive = location.pathname === fullPath

                    return(
                        <Link
                            key={item.path}
                            to={fullPath}
                            className={cn(
                                "flex flex-col items-center gap-1 px-3 py-1.5 rounded-lg min-w-[48px] transition-colors",
                                isActive
                                    ? "text-secondary"
                                    : "text-muted-foreground"
                            )}>
                                <span className="text-xl">{item.icon}</span>
                                <span className="text-xs">{item.label}</span>
                            </Link>
                    )
                })}
            </div>
        </nav>
    );
}