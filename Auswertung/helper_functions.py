# Helper functions
import matplotlib.pyplot as plt
from matplotlib.patches import Patch
import matplotlib.colors as mcolors
import pandas as pd
import numpy as np

from keys import *

TABLEAU_COLORS = list(mcolors.TABLEAU_COLORS) # type: ignore
CSS4_COLORS = list(mcolors.CSS4_COLORS.keys()) # type: ignore
COL_2v2 = ["darkcyan", "cyan", "gold", "yellow"]
COL_3v3 = ["steelblue", "darkcyan", "cyan", "darkorange", "orange", "gold"]

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
                    title="Default title",
                    ylabel="1 to 5 Scale",
                    rot=50,
                    width=0.6,
                    min=None,
                    max=None,
                    colours = TABLEAU_COLORS,
                    n=None,
                    ):
    if not labels:
        labels=list(datasets[0])
    if not n:
        n = len(datasets[0])

    # Set x-positions for boxes
    x_pos_range = np.arange(len(datasets)) / (len(datasets) - 1)
    x_pos = (x_pos_range * 0.5) + 0.75
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
        )
        # Fill the boxes with colours (requires patch_artist=True)
        k = i % len(colours)
        for box in bp['boxes']:
            box.set(facecolor=colours[k])
        # Make the median lines more visible
        plt.setp(bp['medians'], color='black')


    # Titles
    # plt.title(title + ", n=" + str(n))
    plt.title(title)
    plt.ylabel(ylabel)
    plt.xlabel('Question')
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

    plt.show()


def dualAxisUeqBoxplot(data, title):
    rot = 40
    fig, ax = plt.subplots()
    ax.boxplot(x=data)
    ax.set_xlabel('Left hand side')
    ax.set_ylabel('1-7 scale')
    ax.set_title("Short User Experience Questionaire | " + title + ", n=" + str(len(data)))
    ax.set_xticklabels(labels=ShortUserExpQ.Names_left, rotation=rot)

    ax2 = ax.twiny()
    ax2.set_xlim(ax.get_xlim())
    ax2.set_xticks(ax.get_xticks())
    ax2.set_xticklabels(labels=ShortUserExpQ.Names_right, rotation=rot)
    ax2.set_xlabel("Right hand side")
    plt.show()
