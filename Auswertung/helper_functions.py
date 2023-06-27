# Helper functions
import matplotlib.pyplot as plt
from matplotlib.patches import Patch
import matplotlib.colors as mcolors
import pandas as pd
import numpy as np

from keys import *


figureSavePath = "figures/"

# refer to https://matplotlib.org/stable/gallery/color/named_colors.html for all available colors
TABLEAU_COLORS = list(mcolors.TABLEAU_COLORS) # type: ignore
CSS4_COLORS = list(mcolors.CSS4_COLORS.keys()) # type: ignore
COL_2v2 = ["darkcyan", "cyan", "gold", "yellow"]
COL_3v3 = ["steelblue", "darkcyan", "cyan", "darkorange", "orange", "gold"]

def COL_2Groups(nInEachGroup):
    return [*['tab:blue']*nInEachGroup, *['tab:orange']*nInEachGroup]
def alt_COL_2Groups(nInEachGroup):
    return [*['palevioletred']*nInEachGroup, *['mediumturquoise']*nInEachGroup]

def makeBoxPlot(data, labels, title, rot=50, ylabel="Trifft zu", save=False, show=True):
    plt.figure()
    plt.boxplot(x=data, labels=labels)
    plt.xticks(rotation=rot)
    plt.title(title + ", n=" + str(len(data)))
    plt.ylabel(ylabel)
    plt.grid(linestyle="--", linewidth=0.3)
    if save:
        plt.savefig(title)
    if show:
        plt.show()  
        
# Based on: https://rowannicholls.github.io/python/graphs/plt_based/boxplots_multiple_groups.html
def groupedBoxPlots(datasets, 
                    groups, 
                    labels=None, 
                    title="",
                    ylabel="1 to 5 Scale",
                    xlabel="Question",
                    rot=50,
                    width=0.6,
                    gapInGroup=0.5,
                    offsetFromLef=0.75,
                    min=None,
                    max=None,
                    colours = TABLEAU_COLORS,
                    n=None,
                    meanMarkerSize=0,
                    doubleX=False,
                    save=False,
                    dpi=300,
                    ):
    if not labels:
        labels=list(datasets[0])
    if not n:
        n = len(datasets[0])
    fig, ax = plt.subplots()


    # Set x-positions for boxes
    x_pos_range = np.arange(len(datasets)) / (len(datasets) - 1)
    # x_pos = (x_pos_range * 0.5) + 0.75
    x_pos = (x_pos_range * gapInGroup) + offsetFromLef
    # Plot
    for i, data in enumerate(datasets):
        positions = [x_pos[i] + j * 1 for j in range(len(data.T))]
        bp = plt.boxplot(
            np.array(data), 
            sym='', 
            whis=[0, 100],  # type: ignore
            labels=labels, 
            patch_artist=True,
            positions=positions,
            widths=width / len(datasets),
            showmeans=True,
            meanprops={'marker':'o',
                       'markerfacecolor':'white', 
                       'markeredgecolor':'black',
                       'markersize':meanMarkerSize},
        )
        # Fill the boxes with colours (requires patch_artist=True)
        k = i % len(colours)
        for box in bp['boxes']:
            box.set(facecolor=colours[k])
        # Make the median lines more visible
        plt.setp(bp['medians'], color='black')
    

    if doubleX:
        ax2 = ax.twiny()
        ax2.set_xlim(ax.get_xlim())
        ax2.set_xticks(ax.get_xticks())
        ax2.set_xticklabels(labels=ShortUserExpQ.Names_right, rotation=rot)

        if not save:
            ax.set_xlabel('Left hand side')
            ax2.set_xlabel("Right hand side")


    # Titles
    # plt.title(title + ", n=" + str(n))
    plt.ylabel(ylabel)
    plt.xlabel(xlabel)
    # Axis ticks and labels
    plt.xticks(np.arange(len(list(datasets[0]))) + 1, rotation=rot)
    plt.minorticks_on()
    plt.tick_params(axis='x', which='minor', bottom=False)
    plt.subplots_adjust(right=0.98)
    # Change the limits of the x-axis
    plt.xlim([0.5, len(list(datasets[0])) + 0.5])
    if min or max:
        plt.ylim(min, max)
    # Legend
    legend_elements = []
    for i in range(len(datasets)):
        j = i % len(groups)
        k = i % len(colours)
        legend_elements.append(Patch(facecolor=colours[k], label=groups[j]))
    plt.legend(handles=legend_elements, fontsize=8)
    # Straight lines
    plt.grid(linestyle="--", linewidth=0.3)
    if save:
        plt.savefig(figureSavePath+title, dpi=dpi, bbox_inches="tight")
    else:
        plt.title(title)
    plt.show()


def dualAxisUeqBoxplot(data, title, save=False):
    rot = 40
    fig, ax = plt.subplots()
    ax.boxplot(x=data)
    ax.set_ylabel('1-7 scale')
    ax.set_xticklabels(labels=ShortUserExpQ.Names_left, rotation=rot)

    ax2 = ax.twiny()
    ax2.set_xlim(ax.get_xlim())
    ax2.set_xticks(ax.get_xticks())
    ax2.set_xticklabels(labels=ShortUserExpQ.Names_right, rotation=rot)

    if not save:
        ax.set_xlabel('Left hand side')
        ax.set_title("Short User Experience Questionaire | " + title + ", n=" + str(len(data)))
        ax2.set_xlabel("Right hand side")

    if save:
        plt.savefig("figures/UQE-"+title, dpi=300, bbox_inches="tight")
    plt.show()
