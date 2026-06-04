-- Mise Development Database Permission Reset
-- Run this after rolling back migrations to 0
-- Usage: psql -U postgres -d mise_dev -f reset_permissions.sql

GRANT ALL PRIVILEGES ON SCHEMA public TO mise_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO mise_user;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO mise_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO mise_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO mise_user;

\echo 'Permissions granted successfully.'