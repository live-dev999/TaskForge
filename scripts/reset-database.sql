-- Script to reset the database for migration
-- This will delete the TaskItems table and migration history

-- Drop the TaskItems table if it exists
DROP TABLE IF EXISTS "TaskItems" CASCADE;

-- Drop migration history (optional - EF Core will recreate it)
-- Uncomment the next line if you want to completely reset migration history
-- DROP TABLE IF EXISTS "__EFMigrationsHistory" CASCADE;

