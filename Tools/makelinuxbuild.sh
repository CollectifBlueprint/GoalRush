SOLUTIONDIR=$1
TARGETDIR=$2

BUILDDIR="${SOLUTIONDIR}/../Build/GoalRush_""$(date +%Y_%m_%d_%H_%M_%S)";

mkdir -p "${BUILDDIR}"
cp -R "${TARGETDIR}"/* "${BUILDDIR}"
rm -rf "${BUILDDIR}/x86" "${BUILDDIR}"/*.mdb
cp -R "${SOLUTIONDIR}/05 - Content/" "${BUILDDIR}"
cp -R "${SOLUTIONDIR}/../License.txt" "${BUILDDIR}"
cp -R "${SOLUTIONDIR}/../BuildIncludeFiles/ReadMe.txt" "${BUILDDIR}"
