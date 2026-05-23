#!/usr/bin/env bash
# Seed HR_Manage PostgreSQL database with sample departments, employees, and users.
#
# Usage:
#   ./scripts/seed.sh              # Docker container (default)
#   ./scripts/seed.sh --local      # Local PostgreSQL on port 5433
#   ./scripts/seed.sh --docker     # Explicit Docker mode
#
# Regenerate password hash (default password Admin@1234):
#   dotnet run --project scripts/tools/PasswordHashGen -- "YourPassword"

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
SQL_FILE="$SCRIPT_DIR/seed-data.sql"

PGHOST="${PGHOST:-127.0.0.1}"
PGPORT="${PGPORT:-5433}"
PGUSER="${PGUSER:-postgres}"
PGPASSWORD="${PGPASSWORD:-Abcd@1234}"
PGDATABASE="${PGDATABASE:-HR_Manage}"
DOCKER_CONTAINER="${DOCKER_CONTAINER:-hr-management-db}"

MODE="docker"
if [[ "${1:-}" == "--local" ]]; then
  MODE="local"
elif [[ "${1:-}" == "--docker" ]]; then
  MODE="docker"
elif [[ "${1:-}" == "-h" || "${1:-}" == "--help" ]]; then
  echo "Usage: $0 [--docker|--local]"
  exit 0
fi

if [[ ! -f "$SQL_FILE" ]]; then
  echo "Error: seed file not found: $SQL_FILE"
  exit 1
fi

echo "==> Seeding database '$PGDATABASE' ($MODE mode)..."

if [[ "$MODE" == "docker" ]]; then
  if ! docker ps --format '{{.Names}}' | grep -qx "$DOCKER_CONTAINER"; then
    echo "Error: container '$DOCKER_CONTAINER' is not running."
    echo "Start stack first: cd $ROOT_DIR && docker compose up -d"
    exit 1
  fi
  docker exec -i "$DOCKER_CONTAINER" psql -v ON_ERROR_STOP=1 -U "$PGUSER" -d "$PGDATABASE" < "$SQL_FILE"
else
  export PGPASSWORD
  psql -v ON_ERROR_STOP=1 -h "$PGHOST" -p "$PGPORT" -U "$PGUSER" -d "$PGDATABASE" -f "$SQL_FILE"
fi

echo ""
echo "==> Seed completed."
echo "    Login: admin@hr.com / Admin@1234"
echo "    App:   http://localhost:5080/Account/Login"
