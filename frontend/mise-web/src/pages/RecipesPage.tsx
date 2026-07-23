import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import { getRecipes } from "@/api/recipeApi";
import { PageHeader } from "@/components/PageHeader";
import { EmptyState } from "@/components/EmptyState";
import { Button } from "@/components/ui/button";
//import { Badge } from "@/components/ui/badge";
import type { RecipeItem } from "@/api/recipeApi";

export default function RecipesPage(){
    const { user, hasPermission } = useAuth()
    const { slug } = useParams<{ slug: string}>()
    const navigate = useNavigate()

    const [recipes, setRecipes] = useState<RecipeItem[]>([])
    const [loading, setLoading] = useState(true)
    const [search, setSearch] = useState('')

    useEffect(() => {
        if(!user?.token) return

        getRecipes(user.token)
            .then(setRecipes)
            .catch(err => console.error('Failed to load recipes:', err))
            .finally(() => setLoading(false))
    }, [user])

    const filtered = recipes.filter(r => 
        r.title.toLowerCase().includes(search.toLowerCase())
    )

    function getStatusColor(status: string) {
        switch (status) {
            case 'published': return 'bg-green-100 text-green-800'
            case 'draft': return 'bg-yellow-100 text-yellow-800'
            default: return 'bg-muted text-muted-foreground'
        }
    }

    if (loading){
        return(
            <div className="flex items-center justify-center py-16">
                <p className="text-muted-foreground">Loading recipes...</p>
            </div>
        )
    }

    return (
        <div>
            <PageHeader
                title="Recipes"
                subtitle={`${recipes.length} recipe${recipes.length !== 1 ? 's' : ''} in your book`}
                action={
                    hasPermission('recipe', 'create') ? (
                        <Button 
                            onClick={() => navigate(`/${slug}/recipes/new`)}
                            //className="bg-primary text-primary-foreground hover:bg-primary/90"
                            variant="default"
                        >
                            New Recipe
                        </Button>
                    ) : undefined
                }
            />

            <div className="mb-6">
                <input
                    type="text"
                    placeholder="Search recipes..."
                    value={search}
                    onChange={e => setSearch(e.target.value)}
                    className="w-full md:w-80 px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring placeholder:text-muted-foreground"
                />
            </div>
            {filtered.length === 0 ? (
                <EmptyState
                    title={search ? 'No recipes match your search' : 'No recipes yet'}
                    description={search ? 'Try a different search term.' : 'Add your first recipe to get started.'}
                    action={
                        hasPermission('recipe', 'create') && !search ? (
                            <Button
                                onClick={() => navigate(`/${slug}/recipes/new`)}
                                className="bg-primary text-primary-foreground hover:bg-primary/90"
                            >
                                New Recipe
                            </Button>
                        ) : undefined
                    }
                />
            ): (
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                    {filtered.map(recipe => (
                        <div
                            key={recipe.recipeId}
                            onClick={() => navigate(`/${slug}/recipes/${recipe.recipeId}`)}
                            className="bg-card rounded-lg p-4 border border-border cursor-pointer hover:border-primary transition-colors"
                        >
                            <div className="flex items-start justify-between gap-2 mb-2">
                                <h3 className="font-medium text-foreground text-sm leading-snug">
                                    {recipe.title}
                                </h3>
                                <span className={`text-xs px-2 py-0.5 rounded-full font-medium flex-shrink-0 ${getStatusColor(recipe.status)}`}>
                                    {recipe.status}
                                </span>
                            </div>
                            {recipe.description && (
                                <p className="text-xs text-muted-foreground line-clamp-2 mb-3">
                                    {recipe.description}
                                </p>
                            )}
                            <p className="text-xs text-muted-foreground">
                                Updated {new Date(recipe.updatedAt).toLocaleDateString()}
                            </p>
                        </div>
                    ))}
                </div>
            )}
        </div>
    )
}